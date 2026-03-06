using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class WarrantyClaimRepository : GenericRepository<WarrantyClaim>, IWarrantyClaimRepository
    {
        private readonly GZoneDbContext _context;
        public WarrantyClaimRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
