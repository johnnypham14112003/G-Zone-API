using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Category;
using GZone.Service.BusinessModels.Response.Category;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;

namespace GZone.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PagedResponse<CategoryResponse>>> GetCategoryListAsync(
            int pageIndex, int pageSize, CategoryQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new CategoryQuery();
            var predicate = PredicateBuilder.New<Category>(true);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower().Trim();
                predicate = predicate.And(c =>
                    c.CategoryName.ToLower().Contains(keyword) ||
                    (c.Description != null && c.Description.ToLower().Contains(keyword)) ||
                    (c.Slug != null && c.Slug.ToLower().Contains(keyword))
                );
            }

            if (query.ParentCategoryId.HasValue)
                predicate = predicate.And(c => c.ParentCategoryId == query.ParentCategoryId.Value);
            if (query.IsActive.HasValue)
                predicate = predicate.And(c => c.IsActive == query.IsActive.Value);

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "name_asc" => q => q.OrderBy(x => x.CategoryName),
                    "name_desc" => q => q.OrderByDescending(x => x.CategoryName),
                    "order_asc" => q => q.OrderBy(x => x.DisplayOrder),
                    "order_desc" => q => q.OrderByDescending(x => x.DisplayOrder),
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // newest (default)
                };

            var repository = _unitOfWork.GetCategoryRepository();
            var categories = await repository.GetPagedAsync(pageIndex, pageSize, predicate, orderBy);
            var totalCount = await repository.CountAsync(predicate);
            var response = categories.Adapt<List<CategoryResponse>>();

            var pagedResponse = new PagedResponse<CategoryResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<CategoryResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.GetCategoryRepository().GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Not found any category match the Id!");
            return ApiResponse<CategoryResponse>.Success(category.Adapt<CategoryResponse>());
        }

        public async Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryRequest request)
        {
            if (request == null)
                throw new BadRequestException("Invalid request!");

            // Check if parent category exists (if provided)
            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await _unitOfWork.GetCategoryRepository()
                    .AnyAsync(c => c.CategoryId == request.ParentCategoryId.Value);
                if (!parentExists)
                    throw new NotFoundException("Parent category not found!");
            }

            // Check duplicate name
            var nameExists = await _unitOfWork.GetCategoryRepository()
                .AnyAsync(c => c.CategoryName.ToLower() == request.CategoryName.ToLower().Trim());
            if (nameExists)
                throw new ConflictException("Category name already exists!");

            var category = request.Adapt<Category>();
            category.CategoryId = Guid.NewGuid();
            category.CreatedAt = DateTime.Now;

            try
            {
                await _unitOfWork.GetCategoryRepository().AddAsync(category);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<CategoryResponse>.Success(category.Adapt<CategoryResponse>());
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<ApiResponse<CategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            if (request == null || id == Guid.Empty)
                throw new BadRequestException("Invalid request!");

            var category = await _unitOfWork.GetCategoryRepository().GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Not found any category match the Id!");

            // Check if parent category exists (if provided & changed)
            if (request.ParentCategoryId.HasValue && request.ParentCategoryId != category.ParentCategoryId)
            {
                if (request.ParentCategoryId.Value == id)
                    throw new BadRequestException("Category cannot be its own parent!");

                var parentExists = await _unitOfWork.GetCategoryRepository()
                    .AnyAsync(c => c.CategoryId == request.ParentCategoryId.Value);
                if (!parentExists)
                    throw new NotFoundException("Parent category not found!");
            }

            // Check duplicate name (excluding self)
            var nameExists = await _unitOfWork.GetCategoryRepository()
                .AnyAsync(c => c.CategoryName.ToLower() == request.CategoryName.ToLower().Trim()
                            && c.CategoryId != id);
            if (nameExists)
                throw new ConflictException("Category name already exists!");

            category.UpdatedAt = DateTime.Now;

            try
            {
                await _unitOfWork.GetCategoryRepository().UpdateAsync(request.Adapt(category));
                await _unitOfWork.CompleteAsync();
                return ApiResponse<CategoryResponse>.Success(category.Adapt<CategoryResponse>());
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.GetCategoryRepository().GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Not found any category match the Id!");

            // Check if category has sub-categories
            var hasChildren = await _unitOfWork.GetCategoryRepository()
                .AnyAsync(c => c.ParentCategoryId == id);
            if (hasChildren)
                throw new ConflictException("Cannot delete category that has sub-categories! Remove sub-categories first.");

            try
            {
                await _unitOfWork.GetCategoryRepository().DeleteAsync(category);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
