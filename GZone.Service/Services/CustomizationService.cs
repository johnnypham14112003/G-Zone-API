using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Customization;
using GZone.Service.BusinessModels.Response.Customization;
using GZone.Service.BusinessModels.Response.Product;
using GZone.Service.Extensions.Exceptions;
using LinqKit;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GZone.Service.Services
{
    public class CustomizationService //: ICustomizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<CustomizationResponse>> GetCustomizationByIdAsync(Guid id)
        {
            var custom = await _unitOfWork.GetCustomizationRepository().GetOneAsync(
                x => x.CustomId == id,
                include:query => query
                .Include(customization => customization.Customer)
                .Include(customization => customization.Product)
                .Include(customization => customization.ConfirmedByStaffId),
                hasTrackings: false);

            if (custom == null)
            {
                throw new NotFoundException("Not found any product match the Id!");
            }

            var result = custom.Adapt<CustomizationResponse>();

            result.CustomerName = custom.Customer.FullName;
            result.ProductName = custom.Product.ProductName;

            return ApiResponse<CustomizationResponse>.Success(result);
        }

        public async Task<ApiResponse<PagedResponse<CustomizationResponse>>> GetCustomizationListAsync(
        int pageIndex,
        int pageSize,
        CustomizationQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new CustomizationQuery();

            var predicate = PredicateBuilder.New<Customization>(true);

            // ===== SEARCH =====
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower().Trim();

                predicate = predicate.And(c =>
                    c.Name.ToLower().Contains(keyword) ||
                    c.Sku.ToLower().Contains(keyword) ||
                    c.Color.ToLower().Contains(keyword) ||
                    c.Size.ToLower().Contains(keyword)
                );
            }

            // ===== FILTERS =====
            if (query.CustomerId.HasValue)
                predicate = predicate.And(c => c.CustomerId == query.CustomerId.Value);

            if (query.ProductId.HasValue)
                predicate = predicate.And(c => c.ProductId == query.ProductId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                predicate = predicate.And(c => c.Status == query.Status);

            if (query.FromDate.HasValue)
                predicate = predicate.And(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                predicate = predicate.And(c => c.CreatedAt <= query.ToDate.Value);

            // ===== SORTING =====
            Func<IQueryable<Customization>, IOrderedQueryable<Customization>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "price_asc" => q => q.OrderBy(x => x.QuotedPrice),
                    "price_desc" => q => q.OrderByDescending(x => x.QuotedPrice),
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    _ => q => q.OrderByDescending(x => x.CreatedAt)
                };

            var repository = _unitOfWork.GetCustomizationRepository();

            var customizations = await repository.GetPagedAsync(
                pageIndex,
                pageSize,
                predicate,
                orderBy,
                q => q
                    .Include(x => x.Customer)
                    .Include(x => x.Product)
                    .Include(x => x.Staff)
            );

            var totalCount = await repository.CountAsync(predicate);

            var response = customizations.Adapt<List<CustomizationResponse>>();

            var pagedResponse = new PagedResponse<CustomizationResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<CustomizationResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<CustomizationResponse>> CreateCustomizationAsync(
        CustomizationCreateRequest request)
        {
            var repository = _unitOfWork.GetCustomizationRepository();

            var customization = request.Adapt<Customization>();

            customization.Status = "Pending";
            customization.QuotedPrice = 0;
            customization.CreatedAt = DateTime.Now;

            await repository.AddAsync(customization);
            await _unitOfWork.CompleteAsync();

            var result = await repository.GetOneAsync(
                x => x.CustomId == customization.CustomId,
                q => q
                    .Include(x => x.Customer)
                    .Include(x => x.Product)
            );

            var response = result.Adapt<CustomizationResponse>();

            return ApiResponse<CustomizationResponse>.Success(response);
        }

        public async Task<ApiResponse<CustomizationResponse>> UpdateCustomizationAsync(
        Guid id,
        CustomizationUpdateRequest request)
        {
            var repository = _unitOfWork.GetCustomizationRepository();

            var customization = await repository.GetOneAsync(
                x => x.CustomId == id,
                q => q
                    .Include(x => x.Customer)
                    .Include(x => x.Product)
                    .Include(x => x.Staff)
            );

            if (customization == null)
                return ApiResponse<CustomizationResponse>.Failure("Customization not found");

            request.Adapt(customization);

            await repository.UpdateAsync(customization);
            await _unitOfWork.CompleteAsync();

            var response = customization.Adapt<CustomizationResponse>();

            return ApiResponse<CustomizationResponse>.Success(response);
        }

        public async Task<ApiResponse<bool>> DeleteCustomizationAsync(Guid id)
        {
            var repository = _unitOfWork.GetCustomizationRepository();

            var customization = await repository.GetOneAsync(x => x.CustomId == id);

            if (customization == null)
                return ApiResponse<bool>.Failure("Customization not found");

            await repository.DeleteAsync(customization);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true);
        }
    }
}
