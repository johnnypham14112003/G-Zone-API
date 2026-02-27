using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly GZoneDbContext _context;
        public ProductRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
