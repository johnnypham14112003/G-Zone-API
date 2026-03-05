using GZone.Repository.Base;
using GZone.Repository.Models;

namespace GZone.Repository.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetCartByAccountId(Guid accountId);
    }
}