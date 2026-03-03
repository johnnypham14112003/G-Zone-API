using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;

namespace GZone.Service.Interfaces
{
    public interface IOrderVoucherService
    {
        // L?y danh sách voucher ð? áp d?ng vào 1 order
        Task<ApiResponse<List<OrderVoucherResponse>>> GetVouchersByOrderAsync(Guid orderId);

        // L?y danh sách order ð? dùng 1 voucher (Admin/Staff)
        Task<ApiResponse<List<OrderVoucherResponse>>> GetOrdersByVoucherAsync(Guid voucherId);

        // Áp d?ng voucher vào order
        Task<ApiResponse<OrderVoucherResponse>> ApplyVoucherToOrderAsync(OrderVoucherRequest request);

        // G? voucher kh?i order
        Task<ApiResponse<bool>> RemoveVoucherFromOrderAsync(OrderVoucherRequest request);
    }
}
