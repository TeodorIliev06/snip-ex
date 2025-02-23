namespace SnipEx.Web.Infrastructure
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    using static SnipEx.Common.ApplicationConstants;
    using SnipEx.Data.Models;

    public static class ServiceProviderExtensions
    {
        public static async Task SeedAdminAsync(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            bool isRoleExisting = await roleManager
                .RoleExistsAsync(AdminRoleName);
            if (!isRoleExisting)
            {
                await roleManager.CreateAsync(new ApplicationRole(AdminRoleName));
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
                    await userManager.AddToRoleAsync(newAdmin, AdminRoleName);
                }
            }
        }
    }
}
