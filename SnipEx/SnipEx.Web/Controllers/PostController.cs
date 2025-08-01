﻿namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;

    using Common;
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;

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
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var viewModel = new AddPostFormModel();

            await SetLanguagesForPostModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AddPostFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                await SetLanguagesForPostModel(model);
                return View(model);
            }

            var userId = userManager.GetUserId(User)!;
            bool isAdded = await postService.AddPostAsync(model, userId);

            if (!isAdded)
            {
                await SetLanguagesForPostModel(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(id, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = userManager.GetUserId(User);
            var viewModel = await postService.GetPostByIdAsync(postGuid, userId);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        private async Task SetLanguagesForPostModel(AddPostFormModel model)
        {
            model.AvailableLanguages = await languageService.GetAllLanguagesAsync();
        }
    }
}
