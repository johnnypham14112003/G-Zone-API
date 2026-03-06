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
    //Dependency Injection
    private readonly IOrderService _service;

    //Constructor
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

    [Authorize(Roles = "admin")]
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
        var result = await _service.PatchOrderAsync(id, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteOrderAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // ===== OrderDetail sub-resource =====

    [Authorize]
    [HttpGet("{orderId}/details")]
    public async Task<ActionResult<ApiResponse<List<OrderDetailResponse>>>> GetOrderDetails(Guid orderId)
    {
        var result = await _service.GetOrderDetailsAsync(orderId);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("details/{orderDetailId}")]
    public async Task<ActionResult<ApiResponse<OrderDetailResponse>>> GetOrderDetailById(Guid orderDetailId)
    {
        var result = await _service.GetOrderDetailByIdAsync(orderDetailId);
        return StatusCode(result.StatusCode, result);
    }

    // Helper method để lấy ID từ Token
    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null || !Guid.TryParse(idClaim, out Guid accountId))
        {
            throw new UnauthorizedException("Không tìm thấy định danh người dùng hợp lệ.");
        }
        return accountId;
    }
}
