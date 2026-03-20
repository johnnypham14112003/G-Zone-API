using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;
namespace GZone.Repository.Repositories
{
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        private readonly GZoneDbContext _context;
        public ProductVariantRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
