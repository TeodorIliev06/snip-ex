namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Post;
    using Microsoft.AspNetCore.Identity;
    using SnipEx.Data.Models;

    public class PostController(
        IPostService postService,
        UserManager<ApplicationUser> userManager) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(string? tag, string? search, string sort = "newest")
        {
            var viewModel = await postService.GetPostsAsync(tag, search, sort);
            return View(viewModel);
        }

        [HttpGet]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> Create()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AddPostFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var userId = userManager.GetUserId(User)!;
            bool isAdded = await postService.AddPostAsync(model, userId);
            if (!isAdded)
            {
                //Add model errors
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
