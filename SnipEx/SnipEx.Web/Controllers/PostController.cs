namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using SnipEx.Services.Data.Contracts;

    public class PostController(
        IPostService postService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? tag, string? search, string sort = "newest")
        {
            var viewModel = await postService.GetPostsAsync(tag, search, sort);
            return View(viewModel);
        }
    }
}
