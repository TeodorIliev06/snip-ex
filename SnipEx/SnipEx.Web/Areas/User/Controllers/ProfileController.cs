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
        IUserActionService userActionService,
        IPostService postService,
        ICommentService commentService,
        ILanguageService languageService,
        INotificationService notificationService) : Controller
    {
        public async Task<IActionResult> Index(string? id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userId = string.IsNullOrEmpty(id) ? currentUserId : id;

            var profileInformation = await userService.GetProfileInformationAsync(userId);
            var postCards = await postService.GetPostsCardsByIdAsync(userId);

            profileInformation.RecentPosts = postCards;
            profileInformation.IsCurrentUser = userId == currentUserId;
            profileInformation.UserId = userId;
            profileInformation.ConnectionsCount = await userActionService
                .GetConnectionsCountAsync(userId);

            if (!profileInformation.IsCurrentUser)
            {
                profileInformation.IsConnected = await userActionService
                    .DoesConnectionExistAsync(currentUserId, userId);
            }

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

        public async Task<IActionResult> Connections()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var connections = await userService
                .GetUserConnectionsAsync(userId);
            var connectionsCount = await userActionService.GetConnectionsCountAsync(userId);

            //Optimise these queries with a bulk query and dictionary
            foreach (var connection in connections)
            {
                var mutualConnectionsCount = await userActionService
                    .GetMutualConnectionsCountAsync(userId, connection.TargetUserId);
                var totalLikesCount = await userService
                    .GetTotalLikesReceivedByUserAsync(connection.TargetUserId);

                connection.MutualConnectionsCount = mutualConnectionsCount;
                connection.LikesCount = totalLikesCount;
            }

            var viewModel = new UserConnectionsViewModel()
            {
                Connections = connections,
                ConnectionsCount = connectionsCount
            };

            return View(viewModel);
        }
    }
}
