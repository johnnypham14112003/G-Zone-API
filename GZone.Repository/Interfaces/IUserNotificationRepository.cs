using GZone.Repository.Base;
using GZone.Repository.Models;

namespace GZone.Repository.Interfaces
{
    public interface IUserNotificationRepository
        : IGenericRepository<UserNotification>
    {
        Task<IEnumerable<UserNotification>> GetByAccount(Guid accountId);
    }
}