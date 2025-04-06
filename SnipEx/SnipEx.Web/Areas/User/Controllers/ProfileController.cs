using Microsoft.AspNetCore.Mvc;

namespace SnipEx.Web.Areas.User.Controllers
{
    using System.Security.Claims;

    using SnipEx.Services.Data.Contracts;

    [Area("User")]
    public class ProfileController(
        IUserService userService,
        IPostService postService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var profileInformation = await userService.GetProfileInformationAsync(userId);
            var postCards = await postService.GetPostsCardsByIdAsync(userId);

            profileInformation.RecentPosts = postCards;

            return View(profileInformation);
        }

        public IActionResult Notifications()
        {
            return View();
        }
    }
}
