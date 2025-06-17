using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnipEx.Data.Models;

namespace SnipEx.Web.ViewComponents
{
    public class SidePanelViewComponent(UserManager<ApplicationUser> userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                return View("User", user);
            }

            return View("Guest");
        }
    }
}