using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.WarrantyClaim;
using GZone.Service.BusinessModels.Response.WarrantyClaim;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;

namespace GZone.Service.Services
{
    public class WarrantyClaimService : IWarrantyClaimService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarrantyClaimService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PagedResponse<WarrantyClaimResponse>>> GetWarrantyClaimListAsync(
            int pageIndex, int pageSize, WarrantyClaimQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new WarrantyClaimQuery();
            var predicate = PredicateBuilder.New<WarrantyClaim>(true);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower().Trim();
                predicate = predicate.And(w =>
                    w.ClaimNumber.ToLower().Contains(keyword) ||
                    w.IssueDescription.ToLower().Contains(keyword) ||
                    (w.ResolutionNotes != null && w.ResolutionNotes.ToLower().Contains(keyword))
                );
            }

            if (!string.IsNullOrWhiteSpace(query.ClaimStatus))
                predicate = predicate.And(w => w.ClaimStatus == query.ClaimStatus);
            if (!string.IsNullOrWhiteSpace(query.Status))
                predicate = predicate.And(w => w.Status == query.Status);
            if (query.CustomerId.HasValue)
                predicate = predicate.And(w => w.CustomerId == query.CustomerId.Value);
            if (query.ProcessedByStaffId.HasValue)
                predicate = predicate.And(w => w.ProcessedByStaffId == query.ProcessedByStaffId.Value);
            if (query.FromDate.HasValue)
                predicate = predicate.And(w => w.ClaimDate >= query.FromDate.Value);
            if (query.ToDate.HasValue)
                predicate = predicate.And(w => w.ClaimDate <= query.ToDate.Value);

            Func<IQueryable<WarrantyClaim>, IOrderedQueryable<WarrantyClaim>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    "cost_asc" => q => q.OrderBy(x => x.RepairCost),
                    "cost_desc" => q => q.OrderByDescending(x => x.RepairCost),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // newest (default)
                };

            var repository = _unitOfWork.GetWarrantyClaimRepository();
            var claims = await repository.GetPagedAsync(pageIndex, pageSize, predicate, orderBy);
            var totalCount = await repository.CountAsync(predicate);
            var response = claims.Adapt<List<WarrantyClaimResponse>>();

            var pagedResponse = new PagedResponse<WarrantyClaimResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<WarrantyClaimResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<WarrantyClaimResponse>> GetWarrantyClaimByIdAsync(Guid id)
        {
            var claim = await _unitOfWork.GetWarrantyClaimRepository().GetByIdAsync(id);
            if (claim == null)
                throw new NotFoundException("Not found any warranty claim match the Id!");
            return ApiResponse<WarrantyClaimResponse>.Success(claim.Adapt<WarrantyClaimResponse>());
        }

        public async Task<ApiResponse<WarrantyClaimResponse>> CreateWarrantyClaimAsync(WarrantyClaimRequest request)
        {
            if (request == null)
                throw new BadRequestException("Invalid request!");

            var claim = new WarrantyClaim
            {
                ClaimId = Guid.NewGuid(),
                ClaimNumber = $"WC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
                IssueDescription = request.IssueDescription,
                ClaimStatus = "Pending",
                ClaimDate = DateTime.Now,
                RepairCost = 0,
                CreatedAt = DateTime.Now,
                Status = "Active",
                CustomerId = request.CustomerId,
                OrderDetailId = request.OrderDetailId
            };

            try
            {
                await _unitOfWork.GetWarrantyClaimRepository().AddAsync(claim);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<WarrantyClaimResponse>.Success(claim.Adapt<WarrantyClaimResponse>());
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<ApiResponse<WarrantyClaimResponse>> UpdateWarrantyClaimAsync(Guid id, WarrantyClaimUpdateRequest request)
        {
            if (request == null || id == Guid.Empty)
                throw new BadRequestException("Invalid request!");

            var claim = await _unitOfWork.GetWarrantyClaimRepository().GetByIdAsync(id);
            if (claim == null)
                throw new NotFoundException("Not found any warranty claim match the Id!");

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.ClaimStatus))
                claim.ClaimStatus = request.ClaimStatus;
            if (!string.IsNullOrWhiteSpace(request.ResolutionNotes))
                claim.ResolutionNotes = request.ResolutionNotes;
            if (request.RepairCost.HasValue)
                claim.RepairCost = request.RepairCost.Value;
            if (request.ProcessedByStaffId.HasValue)
                claim.ProcessedByStaffId = request.ProcessedByStaffId.Value;
            if (!string.IsNullOrWhiteSpace(request.Status))
                claim.Status = request.Status;

            // If resolved, set resolution date
            if (request.ClaimStatus?.ToLower() == "resolved" && claim.ResolutionDate == null)
                claim.ResolutionDate = DateTime.Now;

            claim.UpdatedAt = DateTime.Now;

            try
            {
                await _unitOfWork.GetWarrantyClaimRepository().UpdateAsync(claim);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<WarrantyClaimResponse>.Success(claim.Adapt<WarrantyClaimResponse>());
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<ApiResponse<bool>> DeleteWarrantyClaimAsync(Guid id)
        {
            var claim = await _unitOfWork.GetWarrantyClaimRepository().GetByIdAsync(id);
            if (claim == null)
                throw new NotFoundException("Not found any warranty claim match the Id!");

            try
            {
                await _unitOfWork.GetWarrantyClaimRepository().DeleteAsync(claim);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
