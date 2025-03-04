namespace SnipEx.Services.Data
{
    using System.Globalization;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mapping;

    using static Common.EntityValidationConstants.Post;

    public class PostService(
        IRepository<Post, Guid> postRepository,
        IRepository<Tag, Guid> tagRepository) : IPostService
    {
        public async Task<PostIndexViewModel> GetPostsAsync(string? tag, string? search, string? sort)
        {
            var query = postRepository.GetAllAttached()
                .Include(p => p.User)
                .Include(p => p.PostsTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(p => p.PostsTags.Any(pt => pt.Tag.Name == tag));
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(p => 
                    p.Title.ToLower().Contains(search) ||
                    p.Content.ToLower().Contains(search));
            }

            query = sort switch
            {
                "popular" => query.OrderByDescending(p => p.Views),
                "rating" => query.OrderByDescending(p => p.Rating),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var posts = await query.Take(20).ToListAsync();
            var popularTags = await tagRepository.GetAllAttached()
                .OrderByDescending(t => t.PostsTags.Count)
                .Take(10)
                .ToListAsync();

            return new PostIndexViewModel
            {
                SearchQuery = search,
                CurrentSort = sort,
                SelectedTag = tag,
                PopularTags = popularTags.Select(t => new TagViewModel
                {
                    Name = t.Name,
                    IsSelected = t.Name == tag
                }).ToList(),
                Posts = posts.Select(p => new PostViewModel
                {
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    Content = p.Content,
                    UserName = p.User?.UserName ?? ApplicationConstants.NoUserName,
                    CreatedAt = p.CreatedAt.ToString(CreatedAtFormat, CultureInfo.InvariantCulture),
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
        }

        public async Task<IEnumerable<PostCardViewModel>> GetFeaturedPostsAsync()
        {
            var featuredPosts = await postRepository.GetAllAttached()
                .Include(p => p.Language)
                .OrderByDescending(p => p.CreatedAt)
                .To<PostCardViewModel>()
                .ToListAsync();

            return featuredPosts;
        }

        public async Task<bool> AddPostAsync(AddPostFormModel model, string userId)
        {
            bool isCreationDateValid = DateTime
                .TryParseExact(model.CreatedAt, CreatedAtFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate);
            if (!isCreationDateValid)
            {
                return false;
            }

            var userGuid = Guid.Parse(userId);

            var post = new Post();
            AutoMapperConfig.MapperInstance.Map(model, post);
            post.CreatedAt = creationDate;
            post.UserId = userGuid; //Workaround

            await postRepository.AddAsync(post);
            await postRepository.SaveChangesAsync();

            return true;
        }
    }
}