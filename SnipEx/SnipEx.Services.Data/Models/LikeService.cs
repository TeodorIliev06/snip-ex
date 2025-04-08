namespace SnipEx.Services.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    public class LikeService(
        IRepository<Post, Guid> postRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IRepository<PostLike, Guid> postLikeRepository,
        IRepository<CommentLike, Guid> commentLikeRepository) : ILikeService
    {
        public async Task<bool> TogglePostLikeAsync(Guid postGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var existingLike = await postLikeRepository
                .FirstOrDefaultAsync(pl =>
                    pl.PostId == postGuid &&
                    pl.UserId == userGuid);

            if (existingLike != null)
            {
                await postLikeRepository.DeleteAsync(existingLike.Id);
                await postLikeRepository.SaveChangesAsync();

                return false;
            }

            var newLike = new PostLike
            {
                Id = Guid.NewGuid(),
                PostId = postGuid,
                UserId = userGuid,
                CreatedAt = DateTime.UtcNow
            };

            await postLikeRepository.AddAsync(newLike);
            await postLikeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsPostLikedByUserAsync(Guid postGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var isPostLiked = await postLikeRepository
                .GetAllAttached()
                .AnyAsync(pl => pl.PostId == postGuid && pl.UserId == userGuid);

            return isPostLiked;
        }

        public async Task<int> GetPostLikesCountAsync(Guid postGuid)
        {
            var postLikesCount = await postLikeRepository
                .GetAllAttached()
                .CountAsync(pl => pl.PostId == postGuid);

            return postLikesCount;
        }

        public async Task<bool> ToggleCommentLikeAsync(Guid commentGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var existingLike = await commentLikeRepository
                .FirstOrDefaultAsync(pl =>
                    pl.CommentId == commentGuid &&
                    pl.UserId == userGuid);

            if (existingLike != null)
            {
                await commentLikeRepository.DeleteAsync(existingLike.Id);
                await commentLikeRepository.SaveChangesAsync();

                return false;
            }

            var newLike = new CommentLike()
            {
                Id = Guid.NewGuid(),
                CommentId = commentGuid,
                UserId = userGuid,
                CreatedAt = DateTime.UtcNow
            };

            await commentLikeRepository.AddAsync(newLike);
            await commentLikeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsCommentLikedByUserAsync(Guid commentGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var isPostLiked = await commentLikeRepository
                .GetAllAttached()
                .AnyAsync(pl => pl.CommentId == commentGuid && pl.UserId == userGuid);

            return isPostLiked;
        }

        public async Task<int> GetCommentLikesCountAsync(Guid commentGuid)
        {
            var postLikesCount = await commentLikeRepository
                .GetAllAttached()
                .CountAsync(pl => pl.CommentId == commentGuid);

            return postLikesCount;
        }

        public async Task<bool> TogglePostSaveAsync(Guid postGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);

            var user = await userRepository
                .GetAllAttached()
                .Include(au => au.Bookmarks)
                .FirstOrDefaultAsync(au => au.Id == userGuid);

            if (user == null)
            {
                //TODO: Throw an appropriate error
                return false;
            }

            var post = await postRepository.GetByIdAsync(postGuid);
            if (post == null)
            {
                //TODO: Throw an appropriate error
                return false;
            }

            var existingBookmark = user.Bookmarks
                .FirstOrDefault(b => b.Id == postGuid);

            if (existingBookmark != null)
            {
                user.Bookmarks.Remove(existingBookmark);
                await userRepository.SaveChangesAsync();

                return false;
            }

            user.Bookmarks.Add(post);
            await userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsPostSavedByUserAsync(Guid postGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);

            var user = await userRepository
                .GetAllAttached()
                .Include(au => au.Bookmarks)
                .FirstOrDefaultAsync(au => au.Id == userGuid);

            return user!.Bookmarks.Any(p => p.Id == postGuid);
        }
    }
}
