using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.WarrantyClaim;
using GZone.Service.BusinessModels.Response.WarrantyClaim;

namespace GZone.Service.Interfaces
{
    public interface IWarrantyClaimService
    {
        Task<ApiResponse<PagedResponse<WarrantyClaimResponse>>> GetWarrantyClaimListAsync(int pageIndex, int pageSize, WarrantyClaimQuery? query);
        Task<ApiResponse<WarrantyClaimResponse>> GetWarrantyClaimByIdAsync(Guid id);
        Task<ApiResponse<WarrantyClaimResponse>> CreateWarrantyClaimAsync(WarrantyClaimRequest request);
        Task<ApiResponse<WarrantyClaimResponse>> UpdateWarrantyClaimAsync(Guid id, WarrantyClaimUpdateRequest request);
        Task<ApiResponse<bool>> DeleteWarrantyClaimAsync(Guid id);
    }
}
