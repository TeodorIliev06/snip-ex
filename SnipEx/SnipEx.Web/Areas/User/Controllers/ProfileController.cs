using Microsoft.AspNetCore.Mvc;

namespace SnipEx.Web.Areas.User.Controllers
{
    using System.Security.Claims;

    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Data.Models;
    using SnipEx.Web.ViewModels.Notification;

    [Area("User")]
    public class ProfileController(
        IUserService userService,
        IPostService postService,
        INotificationService notificationService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var profileInformation = await userService.GetProfileInformationAsync(userId);
            var postCards = await postService.GetPostsCardsByIdAsync(userId);

            profileInformation.RecentPosts = postCards;

            return View(profileInformation);
        }

        public async Task<IActionResult> Notifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var notifications = await notificationService
                .GetUserNotificationsAsync(userId);
            var unreadCount = await notificationService.GetUnreadNotificationsCountAsync(userId);
            var totalCount = await notificationService.GetTotalNotificationsCountAsync(userId);

            var viewModel = new UserNotificationsViewModel()
            {
                Notifications = notifications,
                UnreadCount = unreadCount,
                TotalCount = totalCount,
                HasMoreNotifications = totalCount > notifications.Count(),
                CurrentPage = 1
            };

            return View(viewModel);
        }
    }
}
