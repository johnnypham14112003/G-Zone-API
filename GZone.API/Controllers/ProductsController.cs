using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Request.Product;
using GZone.Service.BusinessModels.Response;
using GZone.Service.BusinessModels.Response.Product;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut()]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Create ([FromBody] ProductRequest input)
        {
            var result = await _productService.CreateProductAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut()]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> Update([FromBody]Guid id, ProductRequest input)
        {
            var result = await _productService.UpdateProductAsync(id, input);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetList([FromBody] ProductListRequest input)
        {
            var result = await _productService.GetProductListAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
