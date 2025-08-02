namespace SnipEx.Web.Areas.User.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SnipEx.Web.ViewModels.User;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Notification;

    using static Common.ApplicationConstants;

    [Area(UserRoleName)]
    [Authorize(Roles = UserRoleName)]
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
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var userId = string.IsNullOrEmpty(id) ? currentUserId : id;

                var profileInformation = await userService.GetProfileInformationAsync(userId);
                var postCards = await postService.GetPostsCardsByIdAsync(userId, 3);

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
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Notifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await notificationService.MarkAllNotificationsAsReadAsync(userId);

            var notifications = await notificationService
                .GetUserNotificationsAsync(userId);
            var totalCount = await notificationService.GetTotalNotificationsCountAsync(userId);

            var viewModel = new UserNotificationsViewModel()
            {
                Notifications = notifications,
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
            var languageDistribution = languageService
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

        public async Task<IActionResult> Connections(int page = 1, int pageSize = 2)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var connectionsCount = await userActionService.GetConnectionsCountAsync(userId);

            var totalPages = (int)Math.Ceiling((double)connectionsCount / pageSize);
            var skip = (page - 1) * pageSize;

            var directConnections = await userService
                .GetUserConnectionsAsync(userId, skip, pageSize);
            var mutualConnections = await userService
                .GetUserMutualConnectionsAsync(userId, skip, pageSize);

            var allConnections = new List<ConnectionViewModel>();
            allConnections.AddRange(directConnections);
            allConnections.AddRange(mutualConnections);

            var targetUserIds = allConnections.Select(c => c.TargetUserId).ToList();

            var mutualConnectionsCounts = await userActionService
                .GetMutualConnectionsCountByUserAsync(userId, targetUserIds);
            var totalLikesCounts = await userService
                .GetTotalLikesReceivedByUserAsync(targetUserIds);

            foreach (var connection in allConnections)
            {
                connection.MutualConnectionsCount = mutualConnectionsCounts
                    .GetValueOrDefault(connection.TargetUserId, 0);
                connection.LikesCount = totalLikesCounts
                    .GetValueOrDefault(connection.TargetUserId, 0);
            }

            var viewModel = new UserConnectionsViewModel()
            {
                Connections = allConnections,
                ConnectionsCount = connectionsCount,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = connectionsCount,
                    TotalPages = totalPages
                }
            };

            return View(viewModel);
        }
    }
}
