using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Customization;
using GZone.Service.BusinessModels.Response;
using GZone.Service.BusinessModels.Response.Customization;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CustomizationController : Controller
{
    private readonly ICustomizationService _service;

    public CustomizationController(ICustomizationService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomizationResponse>>> GetById(Guid id)
    {
        var result = await _service.GetCustomizationByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCustomizationList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] CustomizationQuery? query = null)
    {
        var result = await _service.GetCustomizationListAsync(pageIndex, pageSize, query);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomizationResponse>>> Create(
        [FromBody] CustomizationCreateRequest request)
    {
        var result = await _service.CreateCustomizationAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomizationResponse>>> Update(
        Guid id,
        [FromBody] CustomizationUpdateRequest request)
    {
        var result = await _service.UpdateCustomizationAsync(id, request);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteCustomizationAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
