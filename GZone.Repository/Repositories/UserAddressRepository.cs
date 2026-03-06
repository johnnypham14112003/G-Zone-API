using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
    {
        private readonly GZoneDbContext _context;
        public UserAddressRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
