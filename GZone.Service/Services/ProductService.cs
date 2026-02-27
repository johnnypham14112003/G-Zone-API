using GZone.Repository.Base;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GZone.Service.BusinessModels.Request.Product;
using GZone.Service.BusinessModels.Response.Product;
using GZone.Service.Extensions.Exceptions;
using Mapster;
using GZone.Repository.Models;
using LinqKit;

namespace GZone.Service.Services
{
    public class ProductService //: IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PagedResponse<ProductResponse>>> GetProductListAsync(
    int pageIndex,
    int pageSize,
    ProductListRequest? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new ProductListRequest();

            var predicate = PredicateBuilder.New<Product>(true);

            // ===== SEARCH =====
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower().Trim();

                predicate = predicate.And(p =>
                    p.ProductName.ToLower().Contains(keyword) ||
                    p.Sku.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword))
                );
            }

            // ===== FILTERS =====
            if (query.CategoryId.HasValue)
            {
                predicate = predicate.And(p => p.CategoryId == query.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Brand))
            {
                predicate = predicate.And(p => p.Brand == query.Brand);
            }

            if (query.MinPrice.HasValue)
            {
                predicate = predicate.And(p => p.BasePrice >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                predicate = predicate.And(p => p.BasePrice <= query.MaxPrice.Value);
            }

            if (query.IsActive.HasValue)
            {
                predicate = predicate.And(p => p.IsActive == query.IsActive.Value);
            }

            if (query.IsFeatured.HasValue)
            {
                predicate = predicate.And(p => p.IsFeatured == query.IsFeatured.Value);
            }

            // ===== SORTING =====
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "price_asc" => q => q.OrderBy(x => x.BasePrice),
                    "price_desc" => q => q.OrderByDescending(x => x.BasePrice),
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    "popular" => q => q.OrderByDescending(x => x.ViewCount),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // default newest
                };

            var repository = _unitOfWork.GetProductRepository();

            var products = await repository.GetPagedAsync(
                pageIndex,
                pageSize,
                predicate,
                orderBy);

            var totalCount = await repository.CountAsync(predicate);

            var response = products.Adapt<List<ProductResponse>>();

            var pagedResponse = new PagedResponse<ProductResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<ProductResponse>>.Success(pagedResponse);
        }


        public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.GetProductRepository().GetByIdAsync(id);

            if (product == null)
            {
                throw new NotFoundException("Not found any product match the Id!");
            }
            
            return ApiResponse<ProductResponse>.Success(product.Adapt<ProductResponse>());
        }

        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductRequest request)
        {
            if (request == null)
            {
                throw new BadRequestException("Invalid request!");
            }

            var product = request.Adapt<Product>();

            product.ProductId = Guid.NewGuid();

            try
            {
                await _unitOfWork.GetProductRepository().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<ProductResponse>.Success(product.Adapt<ProductResponse>());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<ProductResponse>> UpdateProductAsync(Guid id, ProductRequest request)
        {
            if (request == null || id == Guid.Empty)
            {
                throw new BadRequestException("Invalid request!");
            }

            var product = await _unitOfWork.GetProductRepository().GetByIdAsync(id);

            if (product == null)
            {
                throw new NotFoundException("Not found any product match the Id!");
            }

            try
            {
                await _unitOfWork.GetProductRepository().UpdateAsync(request.Adapt(product));
                await _unitOfWork.CompleteAsync();
                return ApiResponse<ProductResponse>.Success(product.Adapt<ProductResponse>());
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.GetProductRepository().GetByIdAsync(id);

            if (product == null)
            {
                throw new NotFoundException("Not found any product match the Id!");
            }

            try
            {
                await _unitOfWork.GetProductRepository().DeleteAsync(product);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
