using Asp.Versioning;
using GZone.Service.BusinessModels.Request.ProductVariant;
using GZone.Service.BusinessModels.Response;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductVariantsController : Controller
{
    private readonly IProductVariantService _service;

    public ProductVariantsController(IProductVariantService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ProductVariantQuery? query = null)
    {
        var result = await _service.GetProductVariantListAsync(pageIndex, pageSize, query);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> GetById(Guid id)
    {
        var result = await _service.GetProductVariantByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> Create(ProductVariantCreateRequest request)
    {
        var result = await _service.CreateProductVariantAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> Update(
        Guid id,
        ProductVariantUpdateRequest request)
    {
        var result = await _service.UpdateProductVariantAsync(id, request);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteProductVariantAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
