using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Order;
using GZone.Service.BusinessModels.Response.Order;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrdersController : Controller
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> GetById(Guid id)
    {
        var result = await _service.GetOrderByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderQuery? input = null)
    {
        var result = await _service.GetOrdersListAsync(pageNumber, pageSize, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderQuery? input = null)
    {
        var userId = GetCurrentUserId();
        input ??= new OrderQuery();
        input.CustomerId = userId;
        var result = await _service.GetOrdersListAsync(pageNumber, pageSize, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> Create([FromBody] OrderRequest input)
    {
        var userId = GetCurrentUserId();
        var result = await _service.CreateOrderAsync(userId, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Patch(Guid id, [FromBody] OrderPatchRequest input)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();
        var result = await _service.PatchOrderAsync(id, input, userId, userRole);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteOrderAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("{orderId}/details")]
    public async Task<ActionResult<ApiResponse<List<OrderDetailResponse>>>> GetOrderDetails(Guid orderId)
    {
        var result = await _service.GetOrderDetailsAsync(orderId);
        return StatusCode(result.StatusCode, result);
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null || !Guid.TryParse(idClaim, out Guid accountId))
        {
            throw new UnauthorizedException("Cannot resolve current user id from token.");
        }

        return accountId;
    }

    private string GetCurrentUserRole()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new UnauthorizedException("Cannot resolve current user role from token.");
        }

        return role;
    }
}
