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
    public class VouchersController : Controller
    {
        private readonly IVoucherService _service;

        public VouchersController(IVoucherService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResponse<VoucherResponse>>>> GetList(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _service.GetVouchersAsync(pageIndex, pageSize, search, isActive);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> GetById(Guid id)
        {
            var result = await _service.GetVoucherByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [AllowAnonymous]
        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> GetByCode(string code)
        {
            var result = await _service.GetVoucherByCodeAsync(code);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> Create([FromBody] VoucherRequest input)
        {
            var staffId = GetCurrentUserId();
            var result = await _service.CreateVoucherAsync(input, staffId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] VoucherRequest input)
        {
            input.VoucherId = id;
            var result = await _service.UpdateVoucherAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _service.DeleteVoucherAsync(id);
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
