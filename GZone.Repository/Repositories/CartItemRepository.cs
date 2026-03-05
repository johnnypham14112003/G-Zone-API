using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace GZone.Repository.Repositories
{
    public class CartItemRepository
        : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(GZoneDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetCartByAccountId(Guid accountId)
        {
            return await GetListAsync(
                x => x.AccountId == accountId,
                include: q => q.Include(x => x.ProductVariant)
            ) ?? new List<CartItem>();
        }
    }
}