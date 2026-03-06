using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class OrderVoucherRepository : GenericRepository<OrderVoucher>, IOrderVoucherRepository
    {
        private readonly GZoneDbContext _context;

        public OrderVoucherRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }

        // Add any custom methods below
    }
}
