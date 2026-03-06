using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;

namespace GZone.Repository.Repositories
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        private readonly GZoneDbContext _context;
        public ImageRepository(GZoneDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
