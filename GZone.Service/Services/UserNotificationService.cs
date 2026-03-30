using DLL.Interfaces;
using GZone.Repository;
using GZone.Repository.Models;
using GZone.Repository.Repositories;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response.Notification;

namespace DLL.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly UserNotificationRepository _repo;

        public UserNotificationService(GZoneDbContext context)
        {
            _repo = new UserNotificationRepository(context);
        }

        public async Task<ApiResponse<List<UserNotificationResponse>>> GetByAccount(Guid accountId)  
        {
            try
            {
                var notifications = (await _repo.GetByAccount(accountId)).ToList();
                var result = notifications.Select(x => new UserNotificationResponse
                {
                    NotificationId = x.NotificationId,
                    AccountId = x.AccountId,
                    Title = x.Notification?.Title ?? "",
                    Message = x.Notification?.Message ?? "",
                    NotificationType = x.Notification?.NotificationType ?? "System",
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    CreatedAt = x.CreatedAt
                }).OrderByDescending(x => x.CreatedAt).ToList();

                return ApiResponse<List<UserNotificationResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserNotificationResponse>>.Failure($"Error: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> MarkAsRead(Guid accountId, Guid notificationId)
        {
            try
            {
                var notifications = await _repo.GetListAsync(x => x.AccountId == accountId && x.NotificationId == notificationId);
                var notification = notifications?.FirstOrDefault();

                if (notification == null)
                {
                    return ApiResponse<bool>.Failure("Notification not found for this user", 404);
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;

                await _repo.UpdateAsync(notification);
                await _repo.SaveChangeAsync();

                return ApiResponse<bool>.Success(true, "Marked as read");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error: {ex.Message}", 500);
            }
        }
    }
}
