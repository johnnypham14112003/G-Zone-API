using DLL.Interfaces;
using GZone.Repository;
using GZone.Repository.Models;
using GZone.Repository.Repositories;

namespace DLL.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly CartItemRepository _repo;

        public CartItemService(GZoneDbContext context)
        {
            _repo = new CartItemRepository(context);
        }

        public async Task<List<CartItem>> GetCartByUser(Guid accountId)
        {
            return (await _repo.GetCartByAccountId(accountId)).ToList();
        }

        public async Task AddToCart(CartItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than 0");

            await _repo.AddAsync(item);
            await _repo.SaveChangeAsync();
        }

        public async Task Remove(CartItem item)
        {
            await _repo.DeleteAsync(item);
            await _repo.SaveChangeAsync();
        }
    }
}