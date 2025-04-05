using Microsoft.AspNetCore.Mvc;

namespace SnipEx.Web.Areas.User.Controllers
{
    using SnipEx.Services.Data.Contracts;
    using System.Security.Claims;

    [Area("User")]
    public class ProfileController(
        IUserService userService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profileInformation = await userService.GetProfileInformationAsync(userId);

            return View(profileInformation);
        }
    }
}
