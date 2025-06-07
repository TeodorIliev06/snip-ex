namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
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
                .ThenInclude(b => b.Language)  // Post -> Language
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.User)      // Post -> User (separate include chain)
                .Where(u => u.Id == userGuid)
                .SelectMany(u => u.Bookmarks)
                .To<PostCardViewModel>()
                .ToListAsync();

            return viewModel;
        }

        //Quickfix:
        //  Add conditional logic in every projection
        //  Tradeoff is halved storage usage
        public async Task<IEnumerable<ConnectionViewModel>> GetUserConnectionsAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var viewModel = await userConnectionRepository
                .GetAllAttached()
                .Where(uc =>
                    uc.UserId == userGuid || 
                    uc.ConnectedUserId == userGuid)
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

        public async Task<int> GetTotalLikesReceivedByUserAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var totalLikes = await postLikeRepository
                .GetAllAttached()
                .Where(pl => pl.Post.UserId == userGuid)
                .CountAsync();

            return totalLikes;
        }
    }
}
