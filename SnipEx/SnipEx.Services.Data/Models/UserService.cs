namespace SnipEx.Services.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SnipEx.Data.Models;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Web.ViewModels.User;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Services.Mapping;

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
    }
}
