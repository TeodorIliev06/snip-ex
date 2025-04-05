namespace SnipEx.Services.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.User;
    using SnipEx.Services.Data.Contracts;

    public class UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IUserService
    {
        public async Task<ProfileInformationViewModel> GetProfileInformationAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            var viewModel = new ProfileInformationViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                SnippetsCount = user.Posts.Count
            };

            return viewModel;
        }
    }
}
