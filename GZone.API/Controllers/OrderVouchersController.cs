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
    public class OrderVouchersController : Controller
    {
        private readonly IOrderVoucherService _service;

        public OrderVouchersController(IOrderVoucherService service)
        {
            _service = service;
        }

        // L?y danh sách voucher ð? áp d?ng vào 1 order
        [Authorize]
        [HttpGet("order/{orderId:guid}")]
        public async Task<ActionResult<ApiResponse<List<OrderVoucherResponse>>>> GetVouchersByOrder(Guid orderId)
        {
            var result = await _service.GetVouchersByOrderAsync(orderId);
            return StatusCode(result.StatusCode, result);
        }

        // L?y danh sách order ð? dùng 1 voucher (Admin/Staff)
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("voucher/{voucherId:guid}/orders")]
        public async Task<ActionResult<ApiResponse<List<OrderVoucherResponse>>>> GetOrdersByVoucher(Guid voucherId)
        {
            var result = await _service.GetOrdersByVoucherAsync(voucherId);
            return StatusCode(result.StatusCode, result);
        }

        // Áp d?ng voucher vào order
        [Authorize]
        [HttpPost("apply")]
        public async Task<ActionResult<ApiResponse<OrderVoucherResponse>>> ApplyVoucher([FromBody] OrderVoucherRequest input)
        {
            var result = await _service.ApplyVoucherToOrderAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        // G? voucher kh?i order
        [Authorize]
        [HttpDelete("remove")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveVoucher([FromBody] OrderVoucherRequest input)
        {
            var result = await _service.RemoveVoucherFromOrderAsync(input);
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
