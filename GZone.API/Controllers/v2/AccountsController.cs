using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Account;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GZone.API.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AccountsController : Controller
{
    //Dependency Injection
    private readonly IAccountService _service;

    //Constructor
    public AccountsController(IAccountService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<AccountResponse>>> GetMe()
    {
        // Lấy ID từ Token của user đang đăng nhập
        var userId = GetCurrentUserId();
        var result = await _service.GetAccountProfileAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AccountResponse>>> GetById(Guid id)
    {
        var result = await _service.GetAccountProfileAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPut()]
    public async Task<ActionResult<ApiResponse<AccountResponse>>> Update([FromBody] AccountRequest input)
    {
        var userId = GetCurrentUserId();
        input.Id = userId;
        var result = await _service.UpdateAccountAsync(input);
        return StatusCode(result.StatusCode, result);
    }

    // API này dành cho Admin xóa user, hoặc User tự xóa mình
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAccount(Guid id)
    {
        // Nếu chỉ cho phép admin xóa: Thêm [Authorize(Roles = "Admin")]
        var result = await _service.DeleteAccountAsync(id);
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