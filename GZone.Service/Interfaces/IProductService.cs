using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response.Product;
using GZone.Service.BusinessModels.Request.Product;

namespace GZone.Service.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<PagedResponse<ProductResponse>>> GetProductListAsync (int pageIndex, int pageSize, ProductQuery? query);
        Task<ApiResponse<ProductResponse>> GetProductByIdAsync(Guid id);
        Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductRequest request);
        Task<ApiResponse<ProductResponse>> UpdateProductAsync(Guid id, ProductRequest request);
        Task<ApiResponse<bool>> DeleteProductAsync(Guid id);
    }
}
