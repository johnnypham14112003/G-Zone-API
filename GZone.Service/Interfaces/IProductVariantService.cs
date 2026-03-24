using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.ProductVariant;
using GZone.Service.BusinessModels.Response;

namespace GZone.Service.Interfaces
{
    public interface IProductVariantService
    {
        Task<ApiResponse<PagedResponse<ProductVariantResponse>>> GetProductVariantListAsync(
            int pageIndex,
            int pageSize,
            ProductVariantQuery? query);

        Task<ApiResponse<ProductVariantResponse>> GetProductVariantByIdAsync(Guid id);

        Task<ApiResponse<ProductVariantResponse>> CreateProductVariantAsync(
            ProductVariantCreateRequest request);

        Task<ApiResponse<ProductVariantResponse>> UpdateProductVariantAsync(
            Guid id,
            ProductVariantUpdateRequest request);

        Task<ApiResponse<bool>> DeleteProductVariantAsync(Guid id);
    }
}
