using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace GZone.Repository.Repositories
{
    public class RatingCommentRepository
        : GenericRepository<RatingComment>, IRatingCommentRepository
    {
        public RatingCommentRepository(GZoneDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<RatingComment>> GetByProductId(Guid productId)
        {
            return await GetListAsync(
                x => x.ProductId == productId && !x.IsHidden,
                include: q => q.Include(x => x.Customer)
            ) ?? new List<RatingComment>();
        }
    }
}