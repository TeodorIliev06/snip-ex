namespace SnipEx.WebApi.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Common;
    using SnipEx.Services.Data.Contracts;

    [Authorize]
    public class NotificationApiController(
        INotificationService notificationService) : BaseApiController
    {
        [HttpPatch("[action]/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            var isNotificationGuidValid = ValidationUtils.TryGetGuid(notificationId, out Guid notificationGuid);
            if (!isNotificationGuidValid)
            {
                return BadRequest(new { message = "Invalid Post ID." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await notificationService.MarkNotificationAsReadAsync(notificationGuid, userId);

            return Ok(new { success = result });
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await notificationService.MarkAllNotificationsAsReadAsync(userId);

            return Ok(new { success = result });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMoreNotifications(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var pageSize = 10;

            var notifications = await notificationService
                .GetUserNotificationsAsync(userId, page, pageSize);
            var totalCount = await notificationService.GetTotalNotificationsCountAsync(userId);

            var result = new
            {
                notifications = notifications,
                hasMore = (page * pageSize) < totalCount
            };

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ShowNewNotification()
        {
            //TODO: Cache the response for optimisation
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var likesCount = await notificationService.GetUnreadNotificationsCountAsync(userId);

            return Ok(new { likesCount });
        }
    }
}
