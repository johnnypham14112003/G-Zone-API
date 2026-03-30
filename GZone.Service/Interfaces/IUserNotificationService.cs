using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response.Notification;

namespace DLL.Interfaces
{
    public interface IUserNotificationService
    {
        Task<ApiResponse<List<UserNotificationResponse>>> GetByAccount(Guid accountId);

        Task<ApiResponse<bool>> MarkAsRead(Guid accountId, Guid notificationId);
    }
}