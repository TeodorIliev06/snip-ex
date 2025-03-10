﻿namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;

    using Common;
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Comment;


    public class PostController(
        IPostService postService,
        ICommentService commentService,
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

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(id, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = await postService.GetPostByIdAsync(postGuid);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                //TODO: Implement better notification for client validations
                this.ModelState.AddModelError(nameof(model.Content), $"Content length needs to be at least 50 chars");
                return RedirectToAction(nameof(Details), new { id = model.PostId });
            }

            var userId = userManager.GetUserId(User)!;
            bool isAdded = await commentService.AddCommentAsync(model, userId);
            if (!isAdded)
            {
                //Add model errors
                return RedirectToAction(nameof(Details), new { id = model.PostId });
            }

            return RedirectToAction(nameof(Details), new { id = model.PostId });
        }
    }
}
