using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Mapster;
using System.Linq.Expressions;

namespace GZone.Service.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PagedResponse<VoucherResponse>>> GetVouchersAsync(
            int pageIndex, int pageSize, string? search, bool? isActive)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            Expression<Func<Voucher, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                predicate = v => v.VoucherCode.ToLower().Contains(search.ToLower()) ||
                                 v.VoucherName.ToLower().Contains(search.ToLower());
            }

            if (isActive.HasValue)
            {
                Expression<Func<Voucher, bool>> activeFilter = v => v.IsActive == isActive.Value;
                predicate = predicate == null ? activeFilter : predicate.And(activeFilter);
            }

            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy =
                q => q.OrderByDescending(v => v.CreatedAt);

            var vouchers = await _unitOfWork.GetVoucherRepository()
                .GetPagedAsync(pageIndex, pageSize, predicate, orderBy);

            var totalCount = await _unitOfWork.GetVoucherRepository()
                .CountAsync(predicate ?? (v => true));

            var response = vouchers.Adapt<List<VoucherResponse>>();
            var pagedResponse = new PagedResponse<VoucherResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<VoucherResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<VoucherResponse>> GetVoucherByIdAsync(Guid voucherId)
        {
            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(voucherId);

            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the Id!");

            return ApiResponse<VoucherResponse>.Success(voucher.Adapt<VoucherResponse>());
        }

        public async Task<ApiResponse<VoucherResponse>> GetVoucherByCodeAsync(string code)
        {
            var voucher = await _unitOfWork.GetVoucherRepository()
                .GetOneAsync(v => v.VoucherCode.ToLower() == code.ToLower());

            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the code!");

            return ApiResponse<VoucherResponse>.Success(voucher.Adapt<VoucherResponse>());
        }

        public async Task<ApiResponse<VoucherResponse>> CreateVoucherAsync(VoucherRequest request, Guid staffId)
        {
            // 1. Validate dates
            if (request.EndDate <= request.StartDate)
                throw new BadRequestException("EndDate must be after StartDate!");

            // 2. Check duplicate voucher code
            var isCodeExist = await _unitOfWork.GetVoucherRepository()
                .AnyAsync(v => v.VoucherCode.ToLower() == request.VoucherCode.ToLower());

            if (isCodeExist)
                throw new ConflictException("Voucher code already exists!");

            // 3. Create entity
            var newVoucher = new Voucher
            {
                VoucherId = Guid.NewGuid(),
                VoucherCode = request.VoucherCode,
                VoucherName = request.VoucherName,
                Description = request.Description ?? string.Empty,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinOrderAmount = request.MinOrderAmount,
                MaxUsageTotal = request.MaxUsageTotal,
                MaxUsagePerCustomer = request.MaxUsagePerCustomer,
                CurrentUsageCount = 0,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ApplicableTo = request.ApplicableTo ?? string.Empty,
                CreatedAt = DateTime.Now,
                IsActive = request.IsActive,
                CreatedByStaffId = staffId
            };

            // 4. Save
            await _unitOfWork.GetVoucherRepository().AddAsync(newVoucher);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<VoucherResponse>.Success(newVoucher.Adapt<VoucherResponse>(), "Create voucher successfully!");
        }

        public async Task<ApiResponse<bool>> UpdateVoucherAsync(VoucherRequest request)
        {
            // 1. Check exist
            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(request.VoucherId!.Value);
            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the Id!");

            // 2. Validate dates
            if (request.EndDate <= request.StartDate)
                throw new BadRequestException("EndDate must be after StartDate!");

            // 3. Check duplicate code (n?u ð?i code)
            if (!voucher.VoucherCode.Equals(request.VoucherCode, StringComparison.OrdinalIgnoreCase))
            {
                var isCodeTaken = await _unitOfWork.GetVoucherRepository()
                    .AnyAsync(v => v.VoucherCode.ToLower() == request.VoucherCode.ToLower()
                                   && v.VoucherId != request.VoucherId);
                if (isCodeTaken)
                    throw new ConflictException("Voucher code already used by another voucher!");
            }

            // 4. Update fields
            voucher.VoucherCode = request.VoucherCode;
            voucher.VoucherName = request.VoucherName;
            voucher.Description = request.Description ?? voucher.Description;
            voucher.DiscountType = request.DiscountType;
            voucher.DiscountValue = request.DiscountValue;
            voucher.MaxDiscountAmount = request.MaxDiscountAmount;
            voucher.MinOrderAmount = request.MinOrderAmount;
            voucher.MaxUsageTotal = request.MaxUsageTotal;
            voucher.MaxUsagePerCustomer = request.MaxUsagePerCustomer;
            voucher.StartDate = request.StartDate;
            voucher.EndDate = request.EndDate;
            voucher.ApplicableTo = request.ApplicableTo ?? voucher.ApplicableTo;
            voucher.IsActive = request.IsActive;
            voucher.UpdatedAt = DateTime.Now;

            // 5. Save
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Update voucher successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteVoucherAsync(Guid voucherId)
        {
            // 1. Check exist
            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(voucherId);
            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the Id!");

            // 2. Delete
            await _unitOfWork.GetVoucherRepository().DeleteAsync(voucher);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Delete voucher successfully!");
        }
    }
}
