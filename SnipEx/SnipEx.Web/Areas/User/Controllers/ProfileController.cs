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

            var viewModel = new UserNotificationsViewModel()
            {
                Notifications = await notificationService.GetUserNotificationsAsync(userId),
                UnreadCount = await notificationService.GetUnreadNotificationsCountAsync(userId)
            };

            return View(viewModel);
        }
    }
}
