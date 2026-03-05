using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace GZone.Repository.Repositories
{
    public class UserNotificationRepository
        : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(GZoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<UserNotification>> GetByAccount(Guid accountId)
        {
            return await GetListAsync(
                x => x.AccountId == accountId,
                include: q => q.Include(x => x.Notification)
            ) ?? new List<UserNotification>();
        }
    }
}