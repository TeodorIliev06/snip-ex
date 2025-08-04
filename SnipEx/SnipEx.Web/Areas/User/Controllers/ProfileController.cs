namespace SnipEx.Web.Areas.User.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SnipEx.Data.Models.Enums;
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

        public async Task<IActionResult> Notifications(int page = 1, int pageSize = 4, string filter = "all")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await notificationService.MarkAllNotificationsAsReadAsync(userId);

            var skip = (page - 1) * pageSize;
            IEnumerable<NotificationViewModel> notifications;
            int totalCount;

            switch (filter.ToLower())
            {
                case "mention":
                    notifications = await notificationService.GetUserNotificationsByTypesAsync(userId, new[] {NotificationType.Mention}, skip, pageSize);
                    totalCount = await notificationService.GetNotificationsCountByTypesAsync(userId, new[] { NotificationType.Mention});
                    break;
                case "like":
                    var likeTypes = new[] { NotificationType.PostLike, NotificationType.CommentLike };
                    notifications = await notificationService.GetUserNotificationsByTypesAsync(userId, likeTypes, skip, pageSize);
                    totalCount = await notificationService.GetNotificationsCountByTypesAsync(userId, likeTypes);
                    break;
                case "comment":
                    var commentTypes = new[] { NotificationType.PostComment, NotificationType.CommentReply };
                    notifications = await notificationService.GetUserNotificationsByTypesAsync(userId, commentTypes, skip, pageSize);
                    totalCount = await notificationService.GetNotificationsCountByTypesAsync(userId, commentTypes);
                    break;
                case "connection":
                    var connectionTypes = new[] { NotificationType.ConnectionRequest, NotificationType.UserConnection };
                    notifications = await notificationService.GetUserNotificationsByTypesAsync(userId, connectionTypes, skip, pageSize);
                    totalCount = await notificationService.GetNotificationsCountByTypesAsync(userId, connectionTypes);
                    break;
                default: // "all"
                    notifications = await notificationService.GetUserNotificationsAsync(userId, skip, pageSize);
                    totalCount = await notificationService.GetTotalNotificationsCountAsync(userId);
                    break;
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var viewModel = new UserNotificationsViewModel()
            {
                Notifications = notifications,
                TotalCount = await notificationService.GetTotalNotificationsCountAsync(userId),
                FilteredNotificationsCount = totalCount, // Filtered notifications count
                CurrentFilter = filter,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                }
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

        public async Task<IActionResult> Connections(int page = 1, int pageSize = 4, string filter = "all")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var skip = (page - 1) * pageSize;

            IEnumerable<ConnectionViewModel> connections;
            int totalCount;

            switch (filter.ToLower())
            {
                case "connected":
                    connections = await userService.GetUserConnectionsAsync(userId, skip, pageSize);
                    totalCount = await userActionService.GetConnectionsCountAsync(userId);
                    break;

                case "mutual":
                    connections = await userService.GetUserMutualConnectionsAsync(userId, skip, pageSize);
                    totalCount = await userActionService.GetMutualConnectionsCountAsync(userId);
                    break;

                default: // "all"
                    var directConnections = await userService.GetUserConnectionsAsync(userId, skip, pageSize);
                    var mutualConnections = await userService.GetUserMutualConnectionsAsync(userId, skip, pageSize);

                    connections = directConnections.Concat(mutualConnections);

                    var directCount = await userActionService.GetConnectionsCountAsync(userId);
                    var mutualCount = await userActionService.GetMutualConnectionsCountAsync(userId);
                    totalCount = directCount + mutualCount;
                    break;
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var targetUserIds = connections.Select(c => c.TargetUserId).ToList();
            var mutualConnectionsCounts = await userActionService
                .GetMutualConnectionsCountByUserAsync(userId, targetUserIds);
            var totalLikesCounts = await userService
                .GetTotalLikesReceivedByUserAsync(targetUserIds);

            // Update connection details
            foreach (var connection in connections)
            {
                connection.MutualConnectionsCount = mutualConnectionsCounts
                    .GetValueOrDefault(connection.TargetUserId, 0);
                connection.LikesCount = totalLikesCounts
                    .GetValueOrDefault(connection.TargetUserId, 0);
            }

            var viewModel = new UserConnectionsViewModel()
            {
                Connections = connections,
                ConnectionsCount = await userActionService.GetConnectionsCountAsync(userId), // Total connections count
                FilteredConnectionsCount = totalCount, // Filtered connections count
                CurrentFilter = filter,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                }
            };

            return View(viewModel);
        }
    }
}
