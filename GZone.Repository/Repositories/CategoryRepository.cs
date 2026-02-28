using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly GZoneDbContext _context;
        public CategoryRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
