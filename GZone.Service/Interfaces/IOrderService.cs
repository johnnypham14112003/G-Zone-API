using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Order;
using GZone.Service.BusinessModels.Response.Order;

namespace GZone.Service.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(Guid orderId);
        Task<ApiResponse<PagedResponse<OrderResponse>>> GetOrdersListAsync(int pageIndex, int pageSize, OrderQuery? query);
        Task<ApiResponse<OrderResponse>> CreateOrderAsync(Guid customerId, OrderRequest request);
        Task<ApiResponse<bool>> PatchOrderAsync(Guid orderId, OrderPatchRequest request);
        Task<ApiResponse<bool>> DeleteOrderAsync(Guid orderId);

        // OrderDetail sub-resource
        Task<ApiResponse<List<OrderDetailResponse>>> GetOrderDetailsAsync(Guid orderId);
        Task<ApiResponse<OrderDetailResponse>> GetOrderDetailByIdAsync(Guid orderDetailId);
    }
}
