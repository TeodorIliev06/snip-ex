namespace SnipEx.Web.Infrastructure
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using SnipEx.Data.Models;

    using static SnipEx.Common.ApplicationConstants;

    public static class ServiceProviderExtensions
    {
        public static async Task SeedAdminAsync(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            bool isAdminRoleExisting = await roleManager
                .RoleExistsAsync(AdminRoleName);
            if (!isAdminRoleExisting)
            {
                await roleManager.CreateAsync(new ApplicationRole(AdminRoleName));
            }

            bool isUserRoleExisting = await roleManager
                .RoleExistsAsync(UserRoleName);
            if (!isUserRoleExisting)
            {
                await roleManager.CreateAsync(new ApplicationRole(UserRoleName));
            }

            var adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser()
                {
                    UserName = AdminUsername,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, AdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, UserRoleName);
                    await userManager.AddToRoleAsync(newAdmin, AdminRoleName);
                }
            }
        }
    }
}
