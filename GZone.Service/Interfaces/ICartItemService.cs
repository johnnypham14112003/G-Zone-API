using GZone.Repository.Models;

namespace DLL.Interfaces
{
    public interface ICartItemService
    {
        Task<List<CartItem>> GetCartByUser(Guid accountId);

        Task AddToCart(CartItem item);

        Task Remove(CartItem item);
    }
}