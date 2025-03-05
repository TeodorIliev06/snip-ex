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

            //var viewModel = new PostDetailsViewModel
            //{
            //    Id = post.Id,
            //    Title = post.Title,
            //    Content = post.Content,
            //    LanguageName = post.LanguageName,
            //    UserName = post.UserName,
            //    CreatedAt = post.CreatedAt,
            //    Tags = post.Tags.ToList(),
            //    Comments = post.Comments.Select(c => new CommentViewModel
            //    {
            //        Id = c.Id,
            //        Content = c.Content,
            //        UserName = c.UserName,
            //        CreatedAt = c.CreatedAt
            //    }).ToList()
            //};

            return View(viewModel);
        }
    }
}
