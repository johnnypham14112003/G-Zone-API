using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Auth;
using GZone.Service.BusinessModels.Response;
using GZone.Service.BusinessModels.Response.Account;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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
        public async Task<ActionResult<ApiResponse<AccountResponse>>> Register([FromBody] Service.BusinessModels.Request.Auth.RegisterRequest input)
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

        [Authorize]
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var accountId = GetCurrentUserId();
            var result = await _accountService.ChangePasswordAsync(accountId, request);
            return StatusCode(result.StatusCode, result);
        }

        //[AllowAnonymous]
        //[HttpPost("password")]
        //public async Task<IActionResult> ForgotPassword([FromBody] string email)
        //{
        //    var result = await _accountService.ForgotPasswordAsync(email);
        //    return StatusCode(result.StatusCode, result);
        //}

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] AuthTokenRequest request)
        {
            var result = await _accountService.RefreshTokenAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accountId = GetCurrentUserId();
            await _accountService.RevokeRefreshTokenAsync(accountId);
            return Ok(new { message = "Logged out successfully" });
        }

        // Helper method để lấy ID từ Token
        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !Guid.TryParse(idClaim, out Guid accountId))
            {
                throw new UnauthorizedException("Invalid Account ID!");
            }

            return accountId;
        }
    }
}
