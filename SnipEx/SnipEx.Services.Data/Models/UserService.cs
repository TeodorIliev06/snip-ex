namespace SnipEx.Services.Data.Models
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Web.ViewModels.User;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    using static SnipEx.Common.ErrorMessages;

    public class UserService(
        IRepository<PostLike, Guid> postLikeRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IRepository<UserConnection, object> userConnectionRepository) : IUserService
    {
        public async Task<ProfileInformationViewModel> GetProfileInformationAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var user = await userRepository
                .GetAllAttached()
                .Include(u => u.Posts)
                .Where(u => u.Id == userGuid)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format(UserError.UserNotFound, userId));
            }

            var viewModel = AutoMapperConfig.MapperInstance
                .Map<ProfileInformationViewModel>(user);
            return viewModel;
        }

        public async Task<BookmarkViewModel> GetUserBookmarksAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var user = await userRepository
                .GetAllAttached()
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.Language)
                .Where(u => u.Id == userGuid)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format(UserError.UserNotFound, userId));
            }

            var viewModel = AutoMapperConfig.MapperInstance
                .Map<BookmarkViewModel>(user);

            return viewModel;
        }

        public async Task<IEnumerable<PostCardViewModel>> GetUserSnippetsAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var viewModel = await userRepository
                .GetAllAttached()
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.Language)
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.User)
                .Where(u => u.Id == userGuid)
                .SelectMany(u => u.Bookmarks)
                .To<PostCardViewModel>()
                .ToListAsync();

            return viewModel;
        }

        //Quickfix:
        //  Add conditional logic in every projection
        //  Tradeoff is halved storage usage
        public async Task<IEnumerable<ConnectionViewModel>> GetUserConnectionsAsync(string userId,
            int skip = 0, int take = 4)
        {
            var userGuid = Guid.Parse(userId);
            var viewModel = await userConnectionRepository
                .GetAllAttached()
                .Where(uc =>
                    uc.UserId == userGuid || 
                    uc.ConnectedUserId == userGuid)
                .Skip(skip)
                .Take(take)
                .Select(uc => new ConnectionViewModel
                {
                    UserId = uc.UserId.ToString(),
                    ConnectedUserId = uc.ConnectedUserId.ToString(),
                    TargetUserId = uc.UserId == userGuid
                        ? uc.ConnectedUserId.ToString()
                        : uc.UserId.ToString(),
                    ActorAvatar = uc.UserId == userGuid
                        ? "/" + uc.ConnectedUser.ProfilePicturePath
                        : "/" + uc.User.ProfilePicturePath,
                    Username = uc.UserId == userGuid
                        ? uc.ConnectedUser.UserName!
                        : uc.User.UserName!,
                    PostsCount = uc.UserId == userGuid
                        ? uc.ConnectedUser.Posts.Count
                        : uc.User.Posts.Count,
                    Type = uc.Status
                })
                .ToListAsync();

            return viewModel;
        }

        public async Task<IEnumerable<ConnectionViewModel>> GetUserMutualConnectionsAsync(string userId,
            int skip = 0, int take = 4)
        {
            var userGuid = Guid.Parse(userId);

            var userDirectConnections = await userConnectionRepository
                .GetAllAttached()
                .Where(uc => (uc.UserId == userGuid || uc.ConnectedUserId == userGuid)
                             && uc.Status == ConnectionStatus.Accepted)
                .Select(uc => uc.UserId == userGuid ? uc.ConnectedUserId : uc.UserId)
                .ToListAsync();

            // Find people who are:
            // 1. Connected to user's direct connections
            // 2. NOT directly connected to the user
            // 3. NOT the user themselves
            var mutualConnectionUsers = await userConnectionRepository
                .GetAllAttached()
                .Skip(skip)
                .Take(take)
                .Where(uc =>
                    // One person in the connection is from user's direct connections
                    (userDirectConnections.Contains(uc.UserId) || userDirectConnections.Contains(uc.ConnectedUserId)) &&
                    // The other person is not the current user
                    uc.UserId != userGuid && uc.ConnectedUserId != userGuid &&
                    // The other person is not already a direct connection of the user
                    !(userDirectConnections.Contains(uc.UserId) && userDirectConnections.Contains(uc.ConnectedUserId)) &&
                    uc.Status == ConnectionStatus.Accepted)
                .Include(uc => uc.User)
                .Include(uc => uc.ConnectedUser)
                .ToListAsync();

            var mutualUsers = new List<ConnectionViewModel>();
            var processedUserIds = new HashSet<Guid>();

            foreach (var connection in mutualConnectionUsers)
            {
                Guid mutualUserId;
                ApplicationUser mutualUser = null!;

                if (userDirectConnections.Contains(connection.UserId))
                {
                    mutualUserId = connection.ConnectedUserId;
                    mutualUser = connection.ConnectedUser;
                }
                else if (userDirectConnections.Contains(connection.ConnectedUserId))
                {
                    mutualUserId = connection.UserId;
                    mutualUser = connection.User;
                }
                else
                {
                    continue;
                }

                if (processedUserIds.Contains(mutualUserId) ||
                    userDirectConnections.Contains(mutualUserId))
                {
                    continue;
                }

                processedUserIds.Add(mutualUserId);

                mutualUsers.Add(new ConnectionViewModel
                {
                    UserId = userGuid.ToString(),
                    ConnectedUserId = mutualUserId.ToString(),
                    TargetUserId = mutualUserId.ToString(),
                    ActorAvatar = "/" + mutualUser.ProfilePicturePath,
                    Username = mutualUser.UserName!,
                    PostsCount = mutualUser.Posts.Count,
                    LikesCount = 0,
                    MutualConnectionsCount = 0,
                    Type = ConnectionStatus.Mutual
                });
            }

            return mutualUsers;
        }

        public async Task<Dictionary<string, int>> GetTotalLikesReceivedByUserAsync(List<string> userIds)
        {
            var userGuids = userIds.Select(Guid.Parse).ToList();

            var likesData = await postLikeRepository
                .GetAllAttached()
                .Where(pl => pl.Post.UserId.HasValue &&
                             userGuids.Contains(pl.Post.UserId.Value))
                .GroupBy(pl => pl.Post.UserId!.Value)
                .Select(g => 
                    new { UserId = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = userIds.ToDictionary(id => id, id => 0);

            // Create a mapping from Guid back to original string format
            var guidToStringMap = userIds.ToDictionary(Guid.Parse, id => id);

            foreach (var item in likesData)
            {
                if (guidToStringMap.TryGetValue(item.UserId, out string originalStringId))
                {
                    result[originalStringId] = item.Count;
                }
            }
            return result;
        }
    }
}
