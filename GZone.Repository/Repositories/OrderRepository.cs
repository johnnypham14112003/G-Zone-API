using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly GZoneDbContext _context;
        public OrderRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }

        // Add any custom methods below
    }
}
