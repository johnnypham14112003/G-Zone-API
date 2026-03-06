using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;

namespace GZone.Service.Interfaces
{
    public interface IVoucherService
    {
        Task<ApiResponse<PagedResponse<VoucherResponse>>> GetVouchersAsync(int pageIndex, int pageSize, string? search, bool? isActive);
        Task<ApiResponse<VoucherResponse>> GetVoucherByIdAsync(Guid voucherId);
        Task<ApiResponse<VoucherResponse>> GetVoucherByCodeAsync(string code);
        Task<ApiResponse<VoucherResponse>> CreateVoucherAsync(VoucherRequest request, Guid staffId);
        Task<ApiResponse<bool>> UpdateVoucherAsync(VoucherRequest request);
        Task<ApiResponse<bool>> DeleteVoucherAsync(Guid voucherId);
    }
}
