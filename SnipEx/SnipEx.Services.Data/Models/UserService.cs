﻿namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.User;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    public class UserService(
        IRepository<ApplicationUser, Guid> userRepository) : IUserService
    {
        public async Task<ProfileInformationViewModel> GetProfileInformationAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var viewModel = await userRepository
                .GetAllAttached()
                .Include(u => u.Posts)
                .Where(u => u.Id == userGuid)
                .To<ProfileInformationViewModel>()
                .FirstAsync();

            return viewModel;
        }

        public async Task<BookmarkViewModel> GetUserBookmarksAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var viewModel = await userRepository
                .GetAllAttached()
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.Language)
                .Where(u => u.Id == userGuid)
                .To<BookmarkViewModel>()
                .FirstAsync();

            return viewModel;
        }

        public async Task<IEnumerable<PostCardViewModel>> GetUserSnippetsAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var viewModel = await userRepository
                .GetAllAttached()
                .Include(u => u.Bookmarks)
                .ThenInclude(b => b.Language)
                .Where(u => u.Id == userGuid)
                .To<PostCardViewModel>()
                .ToListAsync();

            return viewModel;
        }
    }
}
