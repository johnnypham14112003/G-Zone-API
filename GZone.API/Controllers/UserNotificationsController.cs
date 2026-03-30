using DLL.Interfaces;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Response.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _service;

        public UserNotificationsController(IUserNotificationService service)
        {
            _service = service;
        }

        // [Authorize] // B?t n?u dùng Token th?c t?
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
    }
}
