using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Order;
using GZone.Service.BusinessModels.Response.Order;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GZone.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(Guid orderId)
        {
            // 1. Get order with details
            var order = await _unitOfWork.GetOrderRepository().GetOneAsync(
                o => o.OrderId == orderId,
                include: q => q.Include(o => o.OrderDetails!));

            if (order == null)
                throw new NotFoundException("Not found any order match the Id!");

            // 2. Map to response
            var response = order.Adapt<OrderResponse>();
            return ApiResponse<OrderResponse>.Success(response);
        }

        public async Task<ApiResponse<PagedResponse<OrderResponse>>> GetOrdersListAsync(
            int pageIndex, int pageSize, OrderQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new OrderQuery();

            var predicate = PredicateBuilder.New<Order>(true);

            // ===== SEARCH =====
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower().Trim();
                predicate = predicate.And(o =>
                    o.OrderNumber.ToLower().Contains(searchTerm) ||
                    o.ReceiverName.ToLower().Contains(searchTerm) ||
                    o.ReceiverPhone.ToLower().Contains(searchTerm));
            }

            // ===== FILTERS =====
            if (!string.IsNullOrWhiteSpace(query.Status))
                predicate = predicate.And(o => o.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
                predicate = predicate.And(o => o.PaymentStatus == query.PaymentStatus);

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
                predicate = predicate.And(o => o.PaymentMethod == query.PaymentMethod);

            if (query.CustomerId.HasValue)
                predicate = predicate.And(o => o.CustomerId == query.CustomerId.Value);

            if (query.WholeSale.HasValue)
                predicate = predicate.And(o => o.WholeSale == query.WholeSale.Value);

            // ===== DATE RANGE =====
            if (query.FromDate.HasValue)
                predicate = predicate.And(o => o.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                predicate = predicate.And(o => o.CreatedAt <= query.ToDate.Value);

            // ===== SORTING =====
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    "total_asc" => q => q.OrderBy(x => x.TotalAmount),
                    "total_desc" => q => q.OrderByDescending(x => x.TotalAmount),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // newest (default)
                };

            var repository = _unitOfWork.GetOrderRepository();

            var orders = await repository.GetPagedAsync(
                pageIndex, pageSize, predicate, orderBy);

            var totalCount = await repository.CountAsync(predicate);

            var response = orders.Adapt<List<OrderResponse>>();

            var pagedResponse = new PagedResponse<OrderResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<OrderResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<OrderResponse>> CreateOrderAsync(Guid customerId, OrderRequest request)
        {
            // 1. Validate
            if (request == null)
                throw new BadRequestException("Invalid request!");

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
                throw new BadRequestException("Shipping address is required!");

            if (string.IsNullOrWhiteSpace(request.ReceiverName))
                throw new BadRequestException("Receiver name is required!");

            if (string.IsNullOrWhiteSpace(request.ReceiverPhone))
                throw new BadRequestException("Receiver phone is required!");

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                throw new BadRequestException("Payment method is required!");

            // 2. Check customer exists
            var customerExists = await _unitOfWork.GetAccountRepository().AnyAsync(a => a.Id == customerId);
            if (!customerExists)
                throw new NotFoundException("Customer not found!");

            // 3. Create order entity
            var newOrder = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                CustomerId = customerId,
                WholeSale = request.WholeSale,
                Status = "Pending",
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "Unpaid",
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity ?? string.Empty,
                ShippingDistrict = request.ShippingDistrict ?? string.Empty,
                ShippingWard = request.ShippingWard ?? string.Empty,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Note = request.Note ?? string.Empty,
                TrackingNumber = string.Empty,
                CancellationReason = string.Empty,
                CreatedAt = DateTime.Now,
                Subtotal = 0,
                DiscountAmount = 0,
                ShippingFee = 0,
                TaxAmount = 0,
                TotalAmount = 0
            };

            // 4. Create order details if provided
            if (request.OrderDetails != null && request.OrderDetails.Count > 0)
            {
                newOrder.OrderDetails = new List<OrderDetail>();
                decimal subtotal = 0;
                decimal totalDiscount = 0;

                foreach (var item in request.OrderDetails)
                {
                    var totalPrice = (item.UnitPrice * item.Quantity) - item.DiscountAmount;
                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = newOrder.OrderId,
                        ProductName = item.ProductName ?? string.Empty,
                        VariantInfo = item.VariantInfo ?? string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = item.DiscountAmount,
                        TotalPrice = totalPrice,
                        IsCustomDesign = item.IsCustomDesign,
                        CustomDesignNote = item.CustomDesignNote ?? string.Empty,
                        CustomDesignImage = item.CustomDesignImage ?? string.Empty,
                        Status = "Active",
                        ProductVariantId = item.ProductVariantId,
                        CustomizationId = item.CustomizationId,
                        WarrantyPeriodMonths = item.WarrantyPeriodMonths,
                        CreatedAt = DateTime.Now
                    };

                    newOrder.OrderDetails.Add(orderDetail);
                    subtotal += item.UnitPrice * item.Quantity;
                    totalDiscount += item.DiscountAmount;
                }

                newOrder.Subtotal = subtotal;
                newOrder.DiscountAmount = totalDiscount;
                newOrder.TotalAmount = subtotal - totalDiscount + newOrder.ShippingFee + newOrder.TaxAmount;
            }

            // 5. Save
            await _unitOfWork.GetOrderRepository().AddAsync(newOrder);
            await _unitOfWork.CompleteAsync();

            var response = newOrder.Adapt<OrderResponse>();
            return ApiResponse<OrderResponse>.Success(response, "Create order successfully!");
        }

        public async Task<ApiResponse<bool>> PatchOrderAsync(Guid orderId, OrderPatchRequest request)
        {
            // 1. Check exist
            var order = await _unitOfWork.GetOrderRepository().GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("Not found any order match the Id!");

            // 2. Patch only provided fields
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                order.Status = request.Status;

                // Auto-set timestamps based on status
                if (request.Status == "Cancelled")
                    order.CancelledAt = DateTime.Now;
                else if (request.Status == "Delivered")
                    order.DeliveredAt = DateTime.Now;
            }

            if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
            {
                order.PaymentStatus = request.PaymentStatus;
                if (request.PaymentStatus == "Paid")
                    order.PaymentAt = DateTime.Now;
            }

            if (!string.IsNullOrWhiteSpace(request.CancellationReason))
                order.CancellationReason = request.CancellationReason;

            if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
                order.TrackingNumber = request.TrackingNumber;

            if (request.EstimatedDelivery.HasValue)
                order.EstimatedDelivery = request.EstimatedDelivery;

            if (request.ManagedByStaffId.HasValue)
                order.ManagedByStaffId = request.ManagedByStaffId;

            order.UpdatedAt = DateTime.Now;

            // 3. Save
            await _unitOfWork.CompleteAsync();
            return ApiResponse<bool>.Success(true, "Update order successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteOrderAsync(Guid orderId)
        {
            // 1. Check exist
            var order = await _unitOfWork.GetOrderRepository().GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("Not found any order match the Id!");

            // 2. Delete
            await _unitOfWork.GetOrderRepository().DeleteAsync(order);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Delete order successfully!");
        }

        // ===== OrderDetail sub-resource =====

        public async Task<ApiResponse<List<OrderDetailResponse>>> GetOrderDetailsAsync(Guid orderId)
        {
            // 1. Check order exists
            var orderExists = await _unitOfWork.GetOrderRepository().AnyAsync(o => o.OrderId == orderId);
            if (!orderExists)
                throw new NotFoundException("Not found any order match the Id!");

            // 2. Get order details
            var details = await _unitOfWork.GetOrderDetailRepository().GetListAsync(
                od => od.OrderId == orderId,
                hasTrackings: false);

            var response = details?.Adapt<List<OrderDetailResponse>>() ?? new List<OrderDetailResponse>();
            return ApiResponse<List<OrderDetailResponse>>.Success(response);
        }

        public async Task<ApiResponse<OrderDetailResponse>> GetOrderDetailByIdAsync(Guid orderDetailId)
        {
            var detail = await _unitOfWork.GetOrderDetailRepository().GetByIdAsync(orderDetailId);
            if (detail == null)
                throw new NotFoundException("Not found any order detail match the Id!");

            var response = detail.Adapt<OrderDetailResponse>();
            return ApiResponse<OrderDetailResponse>.Success(response);
        }

        // ===== Helper =====
        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        }
    }
}
