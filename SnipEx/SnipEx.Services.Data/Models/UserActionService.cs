﻿namespace SnipEx.Services.Data.Models
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Data.Models;
    using SnipEx.Data.Models.Enums;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Posts.PostLiked;
    using SnipEx.Services.Mediator.Comments.CommentLiked;
    using SnipEx.Services.Mediator.Profiles.UserConnection;

    public class UserActionService(
        IRepository<Post, Guid> postRepository,
        IRepository<ApplicationUser, Guid> userRepository,
        IRepository<PostLike, Guid> postLikeRepository,
        IRepository<CommentLike, Guid> commentLikeRepository,
        IRepository<UserConnection, object> userConnectionRepository,
        IMediator mediator) : IUserActionService
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

            var postLikedEvent = new PostLikedEvent(postGuid, userGuid);
            await mediator.Publish(postLikedEvent);

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

            var postLikedEvent = new CommentLikedEvent(commentGuid, userGuid);
            await mediator.Publish(postLikedEvent);

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

        public async Task<bool> ToggleConnectionAsync(string targetUserId, string currentUserId)
        {
            var targetUserGuid = Guid.Parse(targetUserId);
            var currentUserGuid = Guid.Parse(currentUserId);

            if (targetUserGuid == currentUserGuid)
            {
                return false;
            }

            var (smallerId, largerId) = OrderUserIds(currentUserGuid, targetUserGuid);

            var existingConnection = await userConnectionRepository
                .FirstOrDefaultAsync(uc =>
                    uc.UserId == smallerId &&
                    uc.ConnectedUserId == largerId);

            if (existingConnection != null)
            {
                await userConnectionRepository.DeleteAsync(existingConnection);
                await userConnectionRepository.SaveChangesAsync();
                return false;
            }

            var newConnection = new UserConnection
            {
                UserId = smallerId,
                ConnectedUserId = largerId,
                ConnectedOn = DateTime.UtcNow,
                Status = ConnectionStatus.Accepted // Always accept until feature rework in future
            };

            await userConnectionRepository.AddAsync(newConnection);
            await userConnectionRepository.SaveChangesAsync();

            var connectionCreatedEvent = new UserConnectionEvent(currentUserGuid, targetUserGuid);
            await mediator.Publish(connectionCreatedEvent);

            return true;
        }

        public async Task<bool> DoesConnectionExistAsync(string currentUserId, string targetUserId)
        {
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuid = Guid.Parse(targetUserId);

            var (smallerId, largerId) = OrderUserIds(currentUserGuid, targetUserGuid);

            return await userConnectionRepository
                .GetAllAttached()
                .AnyAsync(uc => uc.UserId == smallerId && uc.ConnectedUserId == largerId);
        }

        public async Task<int> GetConnectionsCountAsync(string targetUserId)
        {
            var targetUserGuid = Guid.Parse(targetUserId);

            var connectionsCount = await userConnectionRepository
                .GetAllAttached()
                .CountAsync(uc => 
                    uc.UserId == targetUserGuid ||
                    uc.ConnectedUserId == targetUserGuid);

            return connectionsCount;
        }

        public async Task<int> GetMutualConnectionsCountAsync(string targetUserId)
        {
            var targetUserGuid = Guid.Parse(targetUserId);

            var userDirectConnections = await userConnectionRepository
                .GetAllAttached()
                .Where(uc => (uc.UserId == targetUserGuid || uc.ConnectedUserId == targetUserGuid)
                             && uc.Status == ConnectionStatus.Accepted)
                .Select(uc => uc.UserId == targetUserGuid ? uc.ConnectedUserId : uc.UserId)
                .ToListAsync();

            // Find people who are:
            // 1. Connected to user's direct connections
            // 2. NOT directly connected to the user
            // 3. NOT the user themselves
            var mutualConnectionsCount = await userConnectionRepository
                .GetAllAttached()
                .Where(uc =>
                    // One person in the connection is from user's direct connections
                    (userDirectConnections.Contains(uc.UserId) || userDirectConnections.Contains(uc.ConnectedUserId)) &&
                    // The other person is not the current user
                    uc.UserId != targetUserGuid && uc.ConnectedUserId != targetUserGuid &&
                    // The other person is not already a direct connection of the user
                    !(userDirectConnections.Contains(uc.UserId) && userDirectConnections.Contains(uc.ConnectedUserId)) &&
                    uc.Status == ConnectionStatus.Accepted)
                .Select(uc => userDirectConnections.Contains(uc.UserId) ? uc.ConnectedUserId : uc.UserId)
                .Distinct()
                .Where(mutualUserId => !userDirectConnections.Contains(mutualUserId))
                .CountAsync();

            return mutualConnectionsCount;
        }

        public async Task<Dictionary<string, int>> GetMutualConnectionsCountByUserAsync(string currentUserId, List<string> targetUserIds)
        {
            var currentUserGuid = Guid.Parse(currentUserId);
            var targetUserGuids = targetUserIds.Select(Guid.Parse).ToList();

            var allRelevantUserGuids = new List<Guid> { currentUserGuid };
            allRelevantUserGuids.AddRange(targetUserGuids);

            var allConnections = await userConnectionRepository
                .GetAllAttached()
                .Where(uc =>
                    allRelevantUserGuids.Contains(uc.UserId) ||
                    allRelevantUserGuids.Contains(uc.ConnectedUserId))
                .ToListAsync();

            var currentUserConnections = allConnections
                .Where(uc =>
                    uc.UserId == currentUserGuid ||
                    uc.ConnectedUserId == currentUserGuid)
                .Select(uc => uc.UserId == currentUserGuid 
                    ? uc.ConnectedUserId 
                    : uc.UserId)
                .ToHashSet();

            var result = new Dictionary<string, int>();

            var guidToStringMap = targetUserIds.ToDictionary(Guid.Parse, id => id);

            foreach (var targetUserGuid in targetUserGuids)
            {
                var targetUserConnections = allConnections
                    .Where(uc =>
                        uc.UserId == targetUserGuid ||
                        uc.ConnectedUserId == targetUserGuid)
                    .Select(uc => uc.UserId == targetUserGuid
                        ? uc.ConnectedUserId
                        : uc.UserId)
                    .ToHashSet();

                var mutualCount = currentUserConnections.Intersect(targetUserConnections).Count();

                if (guidToStringMap.TryGetValue(targetUserGuid, out string originalStringId))
                {
                    result[originalStringId] = mutualCount;
                }
            }

            return result;
        }

        public async Task<bool> IncrementPostViewsAsync(Guid postGuid, string userId)
        {
            var userGuid = Guid.Parse(userId);
            var post = await postRepository.GetByIdAsync(postGuid);

            if (post == null ||
                post.UserId == userGuid)
            {
                return false;
            }

            post.Views++;
            await postRepository.UpdateAsync(post);
            await postRepository.SaveChangesAsync();

            return true;
        }

        private static (Guid smallerId, Guid largerId) OrderUserIds(Guid user1Guid, Guid user2Guid)
        {
            if (user1Guid.CompareTo(user2Guid) < 0)
            {
                return (user1Guid, user2Guid);
            }
            return (user2Guid, user1Guid);
        }
    }
}
