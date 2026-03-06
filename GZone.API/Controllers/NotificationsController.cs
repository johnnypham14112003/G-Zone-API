using Asp.Versioning;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Notification;
using GZone.Service.BusinessModels.Response.Notification;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotificationList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] NotificationQuery? input = null)
        {
            var result = await _notificationService.GetNotificationListAsync(pageNumber, pageSize, input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NotificationResponse>>> GetById(Guid id)
        {
            var result = await _notificationService.GetNotificationByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<NotificationResponse>>> Create([FromBody] NotificationRequest input)
        {
            var result = await _notificationService.CreateNotificationAsync(input);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
