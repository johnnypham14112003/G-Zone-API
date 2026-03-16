using Asp.Versioning;
using GZone.Service.BusinessModels.Request.UserAddress;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]es")]
public class UserAddressController : Controller
{
    //Dependency Injection
    private readonly IUserAddressService _userAddressService;

    //Constructor
    public UserAddressController(IUserAddressService userAddressService)
    {
        _userAddressService = userAddressService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userAddressService.GetUserAddressByIdAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListByAccount()
    {
        var accountId = GetCurrentUserId();
        var result = await _userAddressService.GetUserAddressesListAsync(accountId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddressRequest request)
    {
        var result = await _userAddressService.CreateUserAddressAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserAddressRequest request)
    {
        var result = await _userAddressService.UpdateUserAddressAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userAddressService.DeleteUserAddressAsync(id);
        return Ok(result);
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
