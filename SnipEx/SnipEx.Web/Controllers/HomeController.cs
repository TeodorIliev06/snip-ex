using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SnipEx.Web.Models;

namespace SnipEx.Web.Controllers
{
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Post;

    public class HomeController(
        IPostService postService,
        ITagService tagService,
        ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            var featuredPosts = await postService.GetFeaturedPostsAsync();
            var trendingTags = await tagService.GetTrendingTagsAsync();

            var viewModel = new PostCarouselViewModel()
            {
                FeaturedPosts = featuredPosts,
                TrendingTags = trendingTags
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
