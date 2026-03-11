using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly GZoneDbContext _context;

        public VoucherRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }

        // Add any custom methods below
    }
}
