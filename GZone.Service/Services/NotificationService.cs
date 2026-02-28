using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Notification;
using GZone.Service.BusinessModels.Response.Notification;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using LinqKit;
using Mapster;

namespace GZone.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<PagedResponse<NotificationResponse>>> GetNotificationListAsync(
            int pageIndex, int pageSize, NotificationQuery? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            query ??= new NotificationQuery();
            var predicate = PredicateBuilder.New<Notification>(true);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.ToLower().Trim();
                predicate = predicate.And(n =>
                    n.Title.ToLower().Contains(keyword) ||
                    n.Message.ToLower().Contains(keyword)
                );
            }

            if (!string.IsNullOrWhiteSpace(query.NotificationType))
                predicate = predicate.And(n => n.NotificationType == query.NotificationType);
            if (query.OrderId.HasValue)
                predicate = predicate.And(n => n.OrderId == query.OrderId.Value);
            if (query.CustomizationId.HasValue)
                predicate = predicate.And(n => n.CustomizationId == query.CustomizationId.Value);
            if (query.WarrantyClaimId.HasValue)
                predicate = predicate.And(n => n.WarrantyClaimId == query.WarrantyClaimId.Value);
            if (query.FromDate.HasValue)
                predicate = predicate.And(n => n.CreatedAt >= query.FromDate.Value);
            if (query.ToDate.HasValue)
                predicate = predicate.And(n => n.CreatedAt <= query.ToDate.Value);

            Func<IQueryable<Notification>, IOrderedQueryable<Notification>> orderBy =
                query.SortBy?.ToLower() switch
                {
                    "oldest" => q => q.OrderBy(x => x.CreatedAt),
                    _ => q => q.OrderByDescending(x => x.CreatedAt) // newest (default)
                };

            var repository = _unitOfWork.GetNotificationRepository();
            var notifications = await repository.GetPagedAsync(pageIndex, pageSize, predicate, orderBy);
            var totalCount = await repository.CountAsync(predicate);
            var response = notifications.Adapt<List<NotificationResponse>>();

            var pagedResponse = new PagedResponse<NotificationResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<NotificationResponse>>.Success(pagedResponse);
        }

        public async Task<ApiResponse<NotificationResponse>> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _unitOfWork.GetNotificationRepository().GetByIdAsync(id);
            if (notification == null)
                throw new NotFoundException("Not found any notification match the Id!");
            return ApiResponse<NotificationResponse>.Success(notification.Adapt<NotificationResponse>());
        }

        public async Task<ApiResponse<NotificationResponse>> CreateNotificationAsync(NotificationRequest request)
        {
            if (request == null)
                throw new BadRequestException("Invalid request!");

            var notification = request.Adapt<Notification>();
            notification.NotificationId = Guid.NewGuid();
            notification.CreatedAt = DateTime.Now;
            notification.OrderId = request.OrderId ?? Guid.Empty;
            notification.CustomizationId = request.CustomizationId ?? Guid.Empty;
            notification.WarrantyClaimId = request.WarrantyClaimId ?? Guid.Empty;

            try
            {
                await _unitOfWork.GetNotificationRepository().AddAsync(notification);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<NotificationResponse>.Success(notification.Adapt<NotificationResponse>());
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<ApiResponse<bool>> DeleteNotificationAsync(Guid id)
        {
            var notification = await _unitOfWork.GetNotificationRepository().GetByIdAsync(id);
            if (notification == null)
                throw new NotFoundException("Not found any notification match the Id!");

            try
            {
                await _unitOfWork.GetNotificationRepository().DeleteAsync(notification);
                await _unitOfWork.CompleteAsync();
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
