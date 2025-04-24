namespace SnipEx.Web.Areas.User.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;

    using SnipEx.Web.ViewModels.User;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Notification;

    [Area("User")]
    public class ProfileController(
        IUserService userService,
        IPostService postService,
        ICommentService commentService,
        ILanguageService languageService,
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

        public async Task<IActionResult> Bookmarks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var viewModel = await userService.GetUserBookmarksAsync(userId);

            return View(viewModel);
        }

        public async Task<IActionResult> MySnippets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var postCards = await postService.GetPostsCardsByIdAsync(userId);
            var receivedComments = await commentService.GetReceivedCommentsCountAsync(userId);
            var languageDistribution =  languageService
                .GetUserPostsLanguagesDistribution(postCards);
            var mostPopularLanguage = languageDistribution.Any() 
                ? languageDistribution.First().Name
                : "None";

            var mySnippetsViewModel = new UserSnippetsViewModel()
            {
                UserSnippets = postCards,
                TotalSnippets = postCards.Count(),
                TotalComments = receivedComments,
                LanguageDistribution = languageDistribution,
                AvailableLanguages = languageDistribution.Select(l => l.Name),
                MostPopularLanguage = mostPopularLanguage
            };

            return View(mySnippetsViewModel);
        }
    }
}
