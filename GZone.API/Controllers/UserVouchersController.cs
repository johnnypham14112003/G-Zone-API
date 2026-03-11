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
    public class UserVouchersController : Controller
    {
        private readonly IUserVoucherService _service;

        public UserVouchersController(IUserVoucherService service)
        {
            _service = service;
        }

        // L?y t?t c? voucher c?a user ðang ðãng nh?p
        [Authorize]
        [HttpGet("my-vouchers")]
        public async Task<ActionResult<ApiResponse<List<UserVoucherResponse>>>> GetMyVouchers()
        {
            var accountId = GetCurrentUserId();
            var result = await _service.GetVouchersByAccountAsync(accountId);
            return StatusCode(result.StatusCode, result);
        }

        // L?y t?t c? voucher c?a 1 user b?t k? (Admin/Staff)
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("account/{accountId:guid}")]
        public async Task<ActionResult<ApiResponse<List<UserVoucherResponse>>>> GetVouchersByAccount(Guid accountId)
        {
            var result = await _service.GetVouchersByAccountAsync(accountId);
            return StatusCode(result.StatusCode, result);
        }

        // L?y danh sách user s? h?u 1 voucher (Admin/Staff)
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("voucher/{voucherId:guid}/accounts")]
        public async Task<ActionResult<ApiResponse<List<UserVoucherResponse>>>> GetAccountsByVoucher(Guid voucherId)
        {
            var result = await _service.GetAccountsByVoucherAsync(voucherId);
            return StatusCode(result.StatusCode, result);
        }

        // T?ng voucher cho user (Admin/Staff)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("assign")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignVoucher([FromBody] UserVoucherRequest input)
        {
            var result = await _service.AssignVoucherToUserAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        // Thu h?i voucher kh?i user (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("revoke")]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeVoucher([FromBody] UserVoucherRequest input)
        {
            var result = await _service.RevokeVoucherFromUserAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !Guid.TryParse(idClaim, out Guid accountId))
            {
                throw new UnauthorizedException("Không t?m th?y ð?nh danh ngý?i dùng h?p l?.");
            }

            return accountId;
        }
    }
}
