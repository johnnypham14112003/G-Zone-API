using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class UserVoucherRepository : GenericRepository<UserVoucher>, IUserVoucherRepository
    {
        private readonly GZoneDbContext _context;

        public UserVoucherRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }

        // Add any custom methods below
    }
}
