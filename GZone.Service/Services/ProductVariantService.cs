using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.ProductVariant;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;
using Microsoft.EntityFrameworkCore;

public class ProductVariantService : IProductVariantService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductVariantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResponse<ProductVariantResponse>>> GetProductVariantListAsync(
        int pageIndex,
        int pageSize,
        ProductVariantQuery? query)
    {
        if (pageIndex <= 0) pageIndex = 1;
        if (pageSize <= 0) pageSize = 10;

        query ??= new ProductVariantQuery();

        var predicate = PredicateBuilder.New<ProductVariant>(true);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.ToLower().Trim();

            predicate = predicate.And(v =>
                v.Sku.ToLower().Contains(keyword) ||
                v.Color.ToLower().Contains(keyword) ||
                v.Size.ToLower().Contains(keyword));
        }

        if (query.ProductId.HasValue)
            predicate = predicate.And(v => v.ProductId == query.ProductId.Value);

        if (query.IsActive.HasValue)
            predicate = predicate.And(v => v.IsActive == query.IsActive.Value);

        Func<IQueryable<ProductVariant>, IOrderedQueryable<ProductVariant>> orderBy =
            query.SortBy?.ToLower() switch
            {
                "price_asc" => q => q.OrderBy(x => x.AdditionalPrice),
                "price_desc" => q => q.OrderByDescending(x => x.AdditionalPrice),
                _ => q => q.OrderByDescending(x => x.CreatedAt)
            };

        var repository = _unitOfWork.GetProductVariantRepository();

        var variants = await repository.GetPagedAsync(
            pageIndex,
            pageSize,
            predicate,
            orderBy,
            q => q.Include(x => x.Product));

        var totalCount = await repository.CountAsync(predicate);

        var response = variants.Adapt<List<ProductVariantResponse>>();

        var pagedResponse = new PagedResponse<ProductVariantResponse>
        {
            DataList = response,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<ProductVariantResponse>>.Success(pagedResponse);
    }

    public async Task<ApiResponse<ProductVariantResponse>> GetProductVariantByIdAsync(Guid id)
    {
        var repository = _unitOfWork.GetProductVariantRepository();

        var variant = await repository.GetOneAsync(
            x => x.VariantId == id,
            q => q.Include(x => x.Product));

        if (variant == null)
            return ApiResponse<ProductVariantResponse>.Failure("Variant not found");

        var response = variant.Adapt<ProductVariantResponse>();

        return ApiResponse<ProductVariantResponse>.Success(response);
    }

    public async Task<ApiResponse<ProductVariantResponse>> CreateProductVariantAsync(ProductVariantCreateRequest request)
    {
        var repository = _unitOfWork.GetProductVariantRepository();

        var variant = request.Adapt<ProductVariant>();

        variant.VariantId = Guid.NewGuid();
        variant.CreatedAt = DateTime.Now;
        variant.IsActive = true;

        await repository.AddAsync(variant);
        await _unitOfWork.CompleteAsync();

        var response = variant.Adapt<ProductVariantResponse>();

        return ApiResponse<ProductVariantResponse>.Success(response);
    }

    public async Task<ApiResponse<ProductVariantResponse>> UpdateProductVariantAsync(Guid id, ProductVariantUpdateRequest request)
    {
        var repository = _unitOfWork.GetProductVariantRepository();

        var variant = await repository.GetOneAsync(x => x.VariantId == id);

        if (variant == null)
            return ApiResponse<ProductVariantResponse>.Failure("Variant not found");

        request.Adapt(variant);
        variant.UpdatedAt = DateTime.Now;

        await repository.UpdateAsync(variant);
        await _unitOfWork.CompleteAsync();

        var response = variant.Adapt<ProductVariantResponse>();

        return ApiResponse<ProductVariantResponse>.Success(response);
    }

    public async Task<ApiResponse<bool>> DeleteProductVariantAsync(Guid id)
    {
        var repository = _unitOfWork.GetProductVariantRepository();

        var variant = await repository.GetOneAsync(x => x.VariantId == id);

        if (variant == null)
            return ApiResponse<bool>.Failure("Variant not found");

        await repository.DeleteAsync(variant);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<bool>.Success(true);
    }
}
