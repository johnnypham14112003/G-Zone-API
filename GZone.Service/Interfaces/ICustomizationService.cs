using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Customization;
using GZone.Service.BusinessModels.Response.Customization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.Interfaces
{
    public interface ICustomizationService
    {
        Task<ApiResponse<PagedResponse<CustomizationResponse>>> GetCustomizationListAsync(
            int pageIndex,
            int pageSize,
            CustomizationQuery? query);

        Task<ApiResponse<CustomizationResponse>> GetCustomizationByIdAsync(Guid id);

        Task<ApiResponse<CustomizationResponse>> CreateCustomizationAsync(
            CustomizationCreateRequest request);

        Task<ApiResponse<CustomizationResponse>> UpdateCustomizationAsync(
            Guid id,
            CustomizationUpdateRequest request);

        Task<ApiResponse<bool>> DeleteCustomizationAsync(Guid id);
    }
}
