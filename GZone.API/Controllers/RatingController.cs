using DLL.Interfaces;
using GZone.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingCommentService _service;

        public RatingController(IRatingCommentService service)
        {
            _service = service;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var data = await _service.GetByProduct(productId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RatingComment rating)
        {
            await _service.Add(rating);
            return Ok("Created");
        }
    }
}