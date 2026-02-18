using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthsController : Controller
    {
        private readonly IAccountService _accountService;

        public AuthsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AccountResponse>>> Register([FromBody] RegisterRequest input)
        {
            var result = await _accountService.CreateAccountAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] AuthRequest input)
        {
            var result = await _accountService.LoginByPasswordAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] AuthTokenRequest request)
        {
            var result = await _accountService.RefreshTokenAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (accountId == null || accountId == Guid.Empty.ToString())
                throw new UnauthorizedException("Invalid Account ID");

            await _accountService.RevokeRefreshTokenAsync(Guid.Parse(accountId));
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
