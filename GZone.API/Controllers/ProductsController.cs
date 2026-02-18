using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : Controller
    {
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById()
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetList()
        {
            return Ok();
        }
    }
}
