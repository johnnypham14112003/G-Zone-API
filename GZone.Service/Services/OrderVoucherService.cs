using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GZone.Service.Services
{
    public class OrderVoucherService : IOrderVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderVoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<OrderVoucherResponse>>> GetVouchersByOrderAsync(Guid orderId)
        {
            var orderExists = await _unitOfWork.GetOrderVoucherRepository()
                .AnyAsync(ov => ov.OrderId == orderId);

            // Ki?m tra order t?n t?i riêng ð? tr? v? l?i chính xác
            // (dùng OrderVoucher repo tránh ph?i inject thêm IOrderRepository)
            var orderVouchers = await _unitOfWork.GetOrderVoucherRepository().GetListAsync(
                ov => ov.OrderId == orderId,
                include: q => q.Include(ov => ov.Voucher).Include(ov => ov.Order));

            if (orderVouchers is null || orderVouchers.Count == 0)
                return ApiResponse<List<OrderVoucherResponse>>.Success([], "No vouchers applied to this order.");

            var response = orderVouchers.Select(MapToResponse).ToList();
            return ApiResponse<List<OrderVoucherResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<OrderVoucherResponse>>> GetOrdersByVoucherAsync(Guid voucherId)
        {
            var voucherExists = await _unitOfWork.GetVoucherRepository().AnyAsync(v => v.VoucherId == voucherId);
            if (!voucherExists)
                throw new NotFoundException("Not found any voucher match the Id!");

            var orderVouchers = await _unitOfWork.GetOrderVoucherRepository().GetListAsync(
                ov => ov.VoucherId == voucherId,
                include: q => q.Include(ov => ov.Voucher).Include(ov => ov.Order));

            if (orderVouchers is null || orderVouchers.Count == 0)
                return ApiResponse<List<OrderVoucherResponse>>.Success([], "No orders found for this voucher.");

            var response = orderVouchers.Select(MapToResponse).ToList();
            return ApiResponse<List<OrderVoucherResponse>>.Success(response);
        }

        public async Task<ApiResponse<OrderVoucherResponse>> ApplyVoucherToOrderAsync(OrderVoucherRequest request)
        {
            // 1. Ki?m tra voucher t?n t?i và c?n hi?u l?c
            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(request.VoucherId);
            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the Id!");

            if (!voucher.IsActive)
                throw new BadRequestException("Voucher is no longer active!");

            if (voucher.EndDate < DateTime.Now)
                throw new BadRequestException("Voucher has expired!");

            if (voucher.CurrentUsageCount >= voucher.MaxUsageTotal)
                throw new BadRequestException("Voucher has reached its maximum usage limit!");

            // 2. Ki?m tra voucher ð? ðý?c áp d?ng vào order này chýa
            var alreadyApplied = await _unitOfWork.GetOrderVoucherRepository()
                .AnyAsync(ov => ov.OrderId == request.OrderId && ov.VoucherId == request.VoucherId);

            if (alreadyApplied)
                throw new ConflictException("This voucher has already been applied to the order!");

            // 3. L?y order ð? tính discount (thông qua navigation t? OrderVoucher khác, ho?c subtotal = 0 n?u order chýa có voucher)
            var existingOrderVoucher = await _unitOfWork.GetOrderVoucherRepository()
                .GetOneAsync(ov => ov.OrderId == request.OrderId,
                    include: q => q.Include(ov => ov.Order));

            decimal orderSubtotal = existingOrderVoucher?.Order?.Subtotal ?? 0;

            // 4. Tính discount th?c t? d?a theo DiscountType
            decimal discountAmount = voucher.DiscountType.ToLower() switch
            {
                "percentage" => Math.Min(
                    orderSubtotal * voucher.DiscountValue / 100,
                    voucher.MaxDiscountAmount),
                "fixed" => Math.Min(voucher.DiscountValue, orderSubtotal),
                _ => voucher.DiscountValue
            };

            // 5. T?o b?n ghi OrderVoucher
            var orderVoucher = new OrderVoucher
            {
                OrderId = request.OrderId,
                VoucherId = request.VoucherId,
                DiscountAmount = discountAmount,
                AppliedAt = DateTime.Now
            };

            await _unitOfWork.GetOrderVoucherRepository().AddAsync(orderVoucher);

            // 6. Tãng CurrentUsageCount c?a voucher
            voucher.CurrentUsageCount += 1;

            await _unitOfWork.CompleteAsync();

            // 7. Load navigation ð? tr? response
            var created = await _unitOfWork.GetOrderVoucherRepository()
                .GetOneAsync(ov => ov.OrderId == request.OrderId && ov.VoucherId == request.VoucherId,
                    include: q => q.Include(ov => ov.Voucher).Include(ov => ov.Order));

            return ApiResponse<OrderVoucherResponse>.Success(MapToResponse(created!), "Voucher applied to order successfully!");
        }

        public async Task<ApiResponse<bool>> RemoveVoucherFromOrderAsync(OrderVoucherRequest request)
        {
            // 1. Ki?m tra b?n ghi t?n t?i
            var orderVoucher = await _unitOfWork.GetOrderVoucherRepository()
                .GetOneAsync(ov => ov.OrderId == request.OrderId && ov.VoucherId == request.VoucherId,
                    include: q => q.Include(ov => ov.Order));

            if (orderVoucher is null)
                throw new NotFoundException("This voucher is not applied to the specified order!");

            // 2. Ch? cho phép g? khi order chýa thanh toán
            if (orderVoucher.Order.PaymentStatus?.ToLower() == "paid")
                throw new BadRequestException("Cannot remove voucher from a paid order!");

            // 3. Xóa và hoàn tr? CurrentUsageCount
            await _unitOfWork.GetOrderVoucherRepository().DeleteAsync(orderVoucher);

            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(request.VoucherId);
            if (voucher != null && voucher.CurrentUsageCount > 0)
                voucher.CurrentUsageCount -= 1;

            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Voucher removed from order successfully!");
        }

        // -------------------------------------------------------------------------
        private static OrderVoucherResponse MapToResponse(OrderVoucher ov) => new()
        {
            OrderId = ov.OrderId,
            VoucherId = ov.VoucherId,
            DiscountAmount = ov.DiscountAmount,
            AppliedAt = ov.AppliedAt,
            VoucherCode = ov.Voucher.VoucherCode,
            VoucherName = ov.Voucher.VoucherName,
            DiscountType = ov.Voucher.DiscountType,
            DiscountValue = ov.Voucher.DiscountValue,
            OrderNumber = ov.Order.OrderNumber,
            OrderTotalAmount = ov.Order.TotalAmount,
            OrderStatus = ov.Order.Status
        };
    }
}
