using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Notification;
using GZone.Service.BusinessModels.Response.Notification;

namespace GZone.Service.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse<PagedResponse<NotificationResponse>>> GetNotificationListAsync(int pageIndex, int pageSize, NotificationQuery? query);
        Task<ApiResponse<NotificationResponse>> GetNotificationByIdAsync(Guid id);
        Task<ApiResponse<NotificationResponse>> CreateNotificationAsync(NotificationRequest request);
        Task<ApiResponse<bool>> DeleteNotificationAsync(Guid id);
    }
}
