using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.PaymentTransaction;
using GZone.Service.BusinessModels.Response.PaymentTransaction;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;

namespace GZone.Service.Services
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PaymentTransactionResponse>> GetTransactionByIdAsync(Guid transactionId)
        {
            // 1. Get transaction
            var transaction = await _unitOfWork.GetPaymentTransactionRepository().GetByIdAsync(transactionId);

            if (transaction == null)
                throw new NotFoundException("Not found any transaction match the Id!");

            // 2. Map to response
            var response = transaction.Adapt<PaymentTransactionResponse>();
            return ApiResponse<PaymentTransactionResponse>.Success(response);
        }

        public async Task<ApiResponse<PagedResponse<PaymentTransactionResponse>>> GetTransactionsListAsync(
            int pageIndex, int pageSize, PaymentTransactionQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new PaymentTransactionQuery();

            var predicate = PredicateBuilder.New<PaymentTransaction>(true);

            // ===== SEARCH =====
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower().Trim();
                predicate = predicate.And(t =>
                    t.TransactionNumber.ToLower().Contains(searchTerm));
            }

            // ===== FILTERS =====
            if (query.OrderId.HasValue)
                predicate = predicate.And(t => t.OrderId == query.OrderId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                predicate = predicate.And(t => t.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
                predicate = predicate.And(t => t.PaymentMethod == query.PaymentMethod);

            if (!string.IsNullOrWhiteSpace(query.PaymentGateway))
                predicate = predicate.And(t => t.PaymentGateway == query.PaymentGateway);

            // ===== DATE RANGE =====
            if (query.FromDate.HasValue)
                predicate = predicate.And(t => t.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                predicate = predicate.And(t => t.CreatedAt <= query.ToDate.Value);

            // ===== SORTING =====
            Func<IQueryable<PaymentTransaction>, IOrderedQueryable<PaymentTransaction>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    "amount_asc" => q => q.OrderBy(x => x.Amount),
                    "amount_desc" => q => q.OrderByDescending(x => x.Amount),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // newest (default)
                };

            var repository = _unitOfWork.GetPaymentTransactionRepository();

            var transactions = await repository.GetPagedAsync(
                pageIndex, pageSize, predicate, orderBy);

            var totalCount = await repository.CountAsync(predicate);

            var response = transactions.Adapt<List<PaymentTransactionResponse>>();

            var pagedResponse = new PagedResponse<PaymentTransactionResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<PaymentTransactionResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<PaymentTransactionResponse>> CreateTransactionAsync(PaymentTransactionRequest request)
        {
            // 1. Validate
            if (request == null)
                throw new BadRequestException("Invalid request!");

            if (request.OrderId == Guid.Empty)
                throw new BadRequestException("Order ID is required!");

            if (request.Amount <= 0)
                throw new BadRequestException("Amount must be greater than 0!");

            // 2. Check order exists
            var orderExists = await _unitOfWork.GetOrderRepository().AnyAsync(o => o.OrderId == request.OrderId);
            if (!orderExists)
                throw new NotFoundException("Not found any order match the OrderId!");

            // 3. Create transaction entity
            var newTransaction = new PaymentTransaction
            {
                TransactionId = Guid.NewGuid(),
                TransactionNumber = GenerateTransactionNumber(),
                OrderId = request.OrderId,
                PaymentMethod = request.PaymentMethod ?? string.Empty,
                PaymentGateway = request.PaymentGateway ?? string.Empty,
                Amount = request.Amount,
                Currency = request.Currency ?? "VND",
                Status = "Pending",
                GatewayResponse = string.Empty,
                TransactionDate = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            // 4. Save
            await _unitOfWork.GetPaymentTransactionRepository().AddAsync(newTransaction);
            await _unitOfWork.CompleteAsync();

            var response = newTransaction.Adapt<PaymentTransactionResponse>();
            return ApiResponse<PaymentTransactionResponse>.Success(response, "Create transaction successfully!");
        }

        public async Task<ApiResponse<bool>> PatchTransactionAsync(Guid transactionId, PaymentTransactionPatchRequest request)
        {
            // 1. Check exist
            var transaction = await _unitOfWork.GetPaymentTransactionRepository().GetByIdAsync(transactionId);
            if (transaction == null)
                throw new NotFoundException("Not found any transaction match the Id!");

            // 2. Patch only provided fields
            if (!string.IsNullOrWhiteSpace(request.Status))
                transaction.Status = request.Status;

            if (!string.IsNullOrWhiteSpace(request.GatewayResponse))
                transaction.GatewayResponse = request.GatewayResponse;

            if (request.VerifiedByStaffId.HasValue)
                transaction.VerifiedByStaffId = request.VerifiedByStaffId.Value;

            // 3. Save
            await _unitOfWork.CompleteAsync();
            return ApiResponse<bool>.Success(true, "Update transaction successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteTransactionAsync(Guid transactionId)
        {
            // 1. Check exist
            var transaction = await _unitOfWork.GetPaymentTransactionRepository().GetByIdAsync(transactionId);
            if (transaction == null)
                throw new NotFoundException("Not found any transaction match the Id!");

            // 2. Delete
            await _unitOfWork.GetPaymentTransactionRepository().DeleteAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Delete transaction successfully!");
        }

        // ===== Helper =====
        private static string GenerateTransactionNumber()
        {
            return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        }
    }
}
