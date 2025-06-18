namespace SnipEx.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authentication;

    using SnipEx.Data.Models;

    using static Common.ApplicationConstants;

    public class SidePanelViewComponent(UserManager<ApplicationUser> userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    // The user ID from the cookie doesn't exist in DB anymore.
                    // Sign the user out to prevent further issues.
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                    return View("Guest");
                }

                var isAdmin = await userManager.IsInRoleAsync(user!, AdminRoleName);

                ViewData["IsAdmin"] = isAdmin;

                return View("User", user);
            }

            return View("Guest");
        }
    }
}