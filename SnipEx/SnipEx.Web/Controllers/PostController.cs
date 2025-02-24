namespace SnipEx.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Web.ViewModels.Tag;

    using static Common.EntityValidationConstants.Post;

    public class PostController(
        SnipExDbContext context) : Controller
    {
        [HttpGet]
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
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
            }

            // Apply sorting
            query = sort switch
            {
                "popular" => query.OrderByDescending(p => p.Views),
                "rating" => query.OrderByDescending(p => p.Rating),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Get popular tags for sidebar
            var popularTags = await context.Tags
                .OrderByDescending(t => t.PostsTags.Count)
                .Take(10)
                .ToListAsync();

            var posts = await query.Take(20).ToListAsync();

            // Create the view model
            var viewModel = new PostIndexViewModel
            {
                SearchQuery = search,
                CurrentSort = sort,
                SelectedTag = tag,
                PopularTags = popularTags.Select(t => new TagViewModel
                {
                    Name = t.Name,
                    IsSelected = t.Name == tag
                }).ToList(),
                Posts = posts
                    .Select(p => new PostViewModel
                    {
                        Id = p.Id.ToString(),
                        Title = p.Title,
                        Content = p.Content,
                        UserName = p.User?.UserName ?? "Anonymous",
                        CreatedAt = p.CreatedAt.ToString(CreatedAtFormat),
                        Views = p.Views,
                        Rating = p.Rating,
                        CommentCount = p.Comments.Count,
                        Tags = p.PostsTags.Select(pt => new TagViewModel
                        {
                            Name = pt.Tag.Name,
                            IsSelected = pt.Tag.Name == tag
                        }).ToList()
                    }).ToList()
            };

            return View(viewModel);
        }
    }
}
