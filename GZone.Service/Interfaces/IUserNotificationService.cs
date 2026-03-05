using GZone.Repository.Models;

namespace DLL.Interfaces
{
    public interface IUserNotificationService
    {
        Task<List<UserNotification>> GetByAccount(Guid accountId);

        Task MarkAsRead(UserNotification notification);
    }
}