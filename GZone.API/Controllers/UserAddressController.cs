using Asp.Versioning;
using GZone.Service.BusinessModels.Request.UserAddress;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserAddressController : Controller
{
    //Dependency Injection
    private readonly IUserAddressService _userAddressService;

    //Constructor
    public UserAddressController(IUserAddressService userAddressService)
    {
        _userAddressService = userAddressService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userAddressService.GetUserAddressByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetListByAccount(Guid accountId)
    {
        // Trong thực tế, accountId thường được lấy từ Token: 
        // Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        var result = await _userAddressService.GetUserAddressesListAsync(accountId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddressRequest request)
    {
        var result = await _userAddressService.CreateUserAddressAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserAddressRequest request)
    {
        // Đảm bảo ID trên URL và Body đồng nhất
        if (id != request.AddressId)
        {
            return BadRequest(new { message = "Id in URL and Body do not match!" });
        }

        var result = await _userAddressService.UpdateUserAddressAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userAddressService.DeleteUserAddressAsync(id);
        return Ok(result);
    }
}
