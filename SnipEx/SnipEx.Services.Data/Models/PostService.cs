namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Tag;
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    using static Common.EntityValidationConstants.DateFormat;

    public class PostService(
        IRepository<Tag, Guid> tagRepository,
        IRepository<Post, Guid> postRepository,
        ITagService tagService,
        ICommentService commentService,
        IUserActionService userActionService) : IPostService
    {
        public async Task<PostIndexViewModel> GetPostsAsync(string? tag, string? search, string? sort)
        {
            var query = postRepository.GetAllAttached()
                .Include(p => p.User)
                .Include(p => p.PostsTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
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
                "likes" => query.OrderByDescending(p => p.Likes.Count),
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
                    CreatedAt = p.CreatedAt.ToString(IsoString),
                    Views = p.Views,
                    LikesCount = p.Likes.Count,
                    CommentCount = p.Comments.Count,
                    Tags = p.PostsTags.Select(pt => new TagViewModel
                    {
                        Name = pt.Tag.Name,
                        IsSelected = pt.Tag.Name == tag
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<IEnumerable<PostCardViewModel>> GetPostsCardsAsync()
        {
            var postsCards = await postRepository.GetAllAttached()
                .Include(p => p.Language)
                .OrderByDescending(p => p.CreatedAt)
                .To<PostCardViewModel>()
                .ToListAsync();

            return postsCards;
        }

        public async Task<IEnumerable<PostCardViewModel>> GetPostsCardsByIdAsync(string userId, int? take = null)
        {
            var userGuid = Guid.Parse(userId);

            IQueryable<Post> query = postRepository.GetAllAttached()
                .Where(p => p.UserId == userGuid)
                .Include(p => p.Language)
                .OrderByDescending(p => p.CreatedAt);

            // We do not change order, just limit the result set
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var postsCards = await query
                .To<PostCardViewModel>()
                .ToListAsync();

            return postsCards;
        }

        public async Task<bool> AddPostAsync(AddPostFormModel model, string userId)
        {
            var userGuid = Guid.Parse(userId);

            await using var transaction = await postRepository.BeginTransactionAsync();
            try
            {
                var post = new Post();
                AutoMapperConfig.MapperInstance.Map(model, post);
                post.CreatedAt = DateTime.UtcNow;
                post.UserId = userGuid;

                await postRepository.AddAsync(post);
                await postRepository.SaveChangesAsync();

                if (model.Tags?.Any() == true)
                {
                    bool tagSuccess = await tagService.AddTagsToPostAsync(model.Tags, post.Id);
                    if (!tagSuccess)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<PostDetailsViewModel?> GetPostByIdAsync(Guid postGuid, string? userId)
        {
            var post = await postRepository.GetAllAttached()
                .Include(p => p.Comments)
                //.ThenInclude(c => c.Replies) We are building it manually
                .ThenInclude(c => c.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .Include(p => p.PostsTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Language)
                .Include(p => p.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postGuid);

            if (post == null)
            {
                return null;
            }

            var viewModel = new PostDetailsViewModel();
            AutoMapperConfig.MapperInstance.Map(post, viewModel);

            var structuredComments = await commentService
                .GetStructuredComments(viewModel.Comments);
            viewModel.Comments = structuredComments.ToList();

            if (!string.IsNullOrEmpty(userId))
            {
                viewModel.IsLikedByCurrentUser = await userActionService.IsPostLikedByUserAsync(post.Id, userId);
                viewModel.IsBookmarkedByCurrentUser = await userActionService.IsPostSavedByUserAsync(post.Id, userId);

                foreach (var comment in viewModel.Comments)
                {
                    commentService.SetUserLikeStatus(comment, post.Comments, userId);
                }
            }

            return viewModel;
        }
    }
}