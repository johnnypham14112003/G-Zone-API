using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.PaymentTransaction;
using GZone.Service.BusinessModels.Response.PaymentTransaction;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentTransactionsController : Controller
{
    //Dependency Injection
    private readonly IPaymentTransactionService _service;

    //Constructor
    public PaymentTransactionsController(IPaymentTransactionService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PaymentTransactionResponse>>> GetById(Guid id)
    {
        var result = await _service.GetTransactionByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PaymentTransactionQuery? input = null)
    {
        var result = await _service.GetTransactionsListAsync(pageNumber, pageSize, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentTransactionResponse>>> Create([FromBody] PaymentTransactionRequest input)
    {
        var result = await _service.CreateTransactionAsync(input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Patch(Guid id, [FromBody] PaymentTransactionPatchRequest input)
    {
        var result = await _service.PatchTransactionAsync(id, input);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteTransactionAsync(id);
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
