using DLL.Interfaces;
using GZone.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUserNotificationService _service;

        public NotificationController(IUserNotificationService service)
        {
            _service = service;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetByAccount(Guid accountId)
        {
            var data = await _service.GetByAccount(accountId);
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> MarkAsRead([FromBody] UserNotification model)
        {
            await _service.MarkAsRead(model);
            return Ok("Updated");
        }
    }
}