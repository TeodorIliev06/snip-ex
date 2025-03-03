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
        ILanguageService languageService,
        UserManager<ApplicationUser> userManager) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(string? tag, string? search, string sort = "newest")
        {
            var viewModel = await postService.GetPostsAsync(tag, search, sort);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var availableLanguages = await languageService.GetAllLanguagesAsync();
            var viewModel = new AddPostFormModel()
            {
                AvailableLanguages = availableLanguages
            };

            return View(viewModel);
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
