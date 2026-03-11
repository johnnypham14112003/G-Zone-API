using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.PaymentTransaction;
using GZone.Service.BusinessModels.Response.PaymentTransaction;

namespace GZone.Service.Interfaces
{
    public interface IPaymentTransactionService
    {
        Task<ApiResponse<PaymentTransactionResponse>> GetTransactionByIdAsync(Guid transactionId);
        Task<ApiResponse<PagedResponse<PaymentTransactionResponse>>> GetTransactionsListAsync(int pageIndex, int pageSize, PaymentTransactionQuery? query);
        Task<ApiResponse<PaymentTransactionResponse>> CreateTransactionAsync(PaymentTransactionRequest request);
        Task<ApiResponse<bool>> PatchTransactionAsync(Guid transactionId, PaymentTransactionPatchRequest request);
        Task<ApiResponse<bool>> DeleteTransactionAsync(Guid transactionId);
    }
}
