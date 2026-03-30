using Asp.Versioning;
using DLL.Interfaces;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _service;

        public UserNotificationsController(IUserNotificationService service)
        {
            _service = service;
        }

        // [Authorize] // B?t n?u d?ng Token th?c t?
        [HttpGet("{accountId}")]
        public async Task<ActionResult<ApiResponse<List<UserNotificationResponse>>>> GetByAccount(Guid accountId)
        {
            var result = await _service.GetByAccount(accountId);
            return StatusCode(result.StatusCode, result);
        }

        // [Authorize]
        [HttpPut("{accountId}/mark-read/{notificationId}")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid accountId, Guid notificationId)
        {
            var result = await _service.MarkAsRead(accountId, notificationId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{accountId}")]
        public async Task<ActionResult<ApiResponse<bool>>> SendNotification(Guid accountId, [FromBody] Models.SendNotificationRequest request)
        {
            var result = await _service.SendNotificationAsync(accountId, request.Title, request.Message, request.Type);
            return StatusCode(result.StatusCode, result);
        }
    }

    namespace Models 
    {
        public class SendNotificationRequest 
        {
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Type { get; set; } = "System";
        }
    }
}



