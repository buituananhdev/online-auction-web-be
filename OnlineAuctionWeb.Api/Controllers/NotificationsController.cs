using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionWeb.Application.Services;

namespace OnlineAuctionWeb.Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        /// <summary>
        /// Get user notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notificationService.GetListNotifications();
            return Ok(notifications);
        }

        /// <summary>
        /// Read notification.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpPatch("{notificationId}/read")]
        public async Task<IActionResult> ReadNotification(int notificationId)
        {
            await _notificationService.ReadNotification(notificationId);
            return Ok();
        }
    }
}
