using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Category;
using GZone.Service.BusinessModels.Response.Category;

namespace GZone.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<PagedResponse<CategoryResponse>>> GetCategoryListAsync(int pageIndex, int pageSize, CategoryQuery? query);
        Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(Guid id);
        Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryRequest request);
        Task<ApiResponse<CategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
    }
}
