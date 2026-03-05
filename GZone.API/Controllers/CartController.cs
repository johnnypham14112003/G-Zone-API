using DLL.Interfaces;
using GZone.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartItemService _service;

        public CartController(ICartItemService service)
        {
            _service = service;
        }

        // GET: api/cart/{accountId}
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetCart(Guid accountId)
        {
            var data = await _service.GetCartByUser(accountId);
            return Ok(data);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            await _service.AddToCart(item);
            return Ok("Added");
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] CartItem item)
        {
            await _service.Remove(item);
            return Ok("Deleted");
        }
    }
}