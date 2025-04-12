namespace SnipEx.WebApi.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Common;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Data.Models;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await notificationService.MarkAllNotificationsAsReadAsync(userId);

            return Ok(new { success = result });
        }
    }
}
