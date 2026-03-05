using DLL.Interfaces;
using GZone.Repository;
using GZone.Repository.Models;
using GZone.Repository.Repositories;

namespace DLL.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly UserNotificationRepository _repo;

        public UserNotificationService(GZoneDbContext context)
        {
            _repo = new UserNotificationRepository(context);
        }

        public async Task<List<UserNotification>> GetByAccount(Guid accountId)
        {
            return (await _repo.GetByAccount(accountId)).ToList();
        }

        public async Task MarkAsRead(UserNotification notification)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;

            await _repo.UpdateAsync(notification);
            await _repo.SaveChangeAsync();
        }
    }
}