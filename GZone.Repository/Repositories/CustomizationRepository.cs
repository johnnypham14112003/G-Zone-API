using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;


namespace GZone.Repository.Repositories
{
    public class CustomizationRepository : GenericRepository<Customization>, ICustomizationRepository
    {
        private readonly GZoneDbContext _context;

        public CustomizationRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
