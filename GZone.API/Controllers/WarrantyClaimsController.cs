using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.WarrantyClaim;
using GZone.Service.BusinessModels.Response.WarrantyClaim;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WarrantyClaimsController : Controller
    {
        private readonly IWarrantyClaimService _warrantyClaimService;

        public WarrantyClaimsController(IWarrantyClaimService warrantyClaimService)
        {
            _warrantyClaimService = warrantyClaimService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetWarrantyClaimList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] WarrantyClaimQuery? input = null)
        {
            var result = await _warrantyClaimService.GetWarrantyClaimListAsync(pageNumber, pageSize, input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<WarrantyClaimResponse>>> GetById(Guid id)
        {
            var result = await _warrantyClaimService.GetWarrantyClaimByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WarrantyClaimResponse>>> Create([FromBody] WarrantyClaimRequest input)
        {
            var result = await _warrantyClaimService.CreateWarrantyClaimAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<WarrantyClaimResponse>>> Update(
            [FromRoute] Guid id,
            [FromBody] WarrantyClaimUpdateRequest input)
        {
            var result = await _warrantyClaimService.UpdateWarrantyClaimAsync(id, input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _warrantyClaimService.DeleteWarrantyClaimAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
