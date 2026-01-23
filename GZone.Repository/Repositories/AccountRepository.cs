using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly GZoneDbContext _context;
        public AccountRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }

        // Add any custom methods below
    }
}
