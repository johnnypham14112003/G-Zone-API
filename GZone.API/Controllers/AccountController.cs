using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : Controller
{
    //Dependency Injection
    private readonly IAccountService _service;

    //Constructor
    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet("example")]
    public async Task<IActionResult> ExampleMethod()
    {
        var result = await _service.LoginWithEmailPasswordAsync("ok","123");
        return Ok();
    }
}