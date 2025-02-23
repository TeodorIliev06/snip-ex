namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data;

    public class PostController(
        SnipExDbContext context) : Controller
    {
        public async Task<IActionResult> Index(string? tag, string? search, string sort = "newest")
        {
            var query = context.Posts
                .Include(p => p.User)
                .Include(p => p.PostsTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(p => p.PostsTags.Any(pt => pt.Tag.Name == tag));
                ViewBag.SelectedTag = tag;
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
                ViewBag.SearchQuery = search;
            }

            // Apply sorting
            query = sort switch
            {
                "popular" => query.OrderByDescending(p => p.Views),
                "rating" => query.OrderByDescending(p => p.Rating),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
            ViewBag.CurrentSort = sort;

            // Get popular tags for sidebar
            ViewBag.PopularTags = await context.Tags
                .OrderByDescending(t => t.PostsTags.Count)
                .Take(10)
                .ToListAsync();

            var posts = await query.Take(20).ToListAsync();
            return View(posts);
        }
    }
}
