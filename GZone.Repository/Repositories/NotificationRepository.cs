using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly GZoneDbContext _context;
        public NotificationRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
