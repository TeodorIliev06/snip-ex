namespace SnipEx.Services.Data
{
    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class LikeService(
        IRepository<PostLike, Guid> postLikeRepository) : ILikeService
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

        public async Task<int> GetPostLikesCountAsync(Guid postId)
        {
            var postLikesCount = await postLikeRepository
                .GetAllAttached()
                .CountAsync(pl => pl.PostId == postId);

            return postLikesCount;
        }
    }
}
