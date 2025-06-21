namespace SnipEx.Services.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Admin;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    public class ManagerService(
        IRepository<ApplicationUser, Guid> userRepository,
        UserManager<ApplicationUser> userManager) : IManagerService
    {
        public async Task<IEnumerable<ManageUserViewModel>> GetAllUsersWithBanStatusAsync()
        {
            var users = await userRepository
                .GetAllAttached()
                .Include(u => u.Posts)
                .OrderBy(u => u.UserName)
                .To<ManageUserViewModel>()
                .ToListAsync();

            return users;
        }

        public async Task<bool> ToggleBanStatusAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var user = await userRepository
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                return false;
            }

            user.IsBanned = !user.IsBanned;

            await userRepository.UpdateAsync(user);
            await userRepository.SaveChangesAsync();

            return true;
        }
    }
}
