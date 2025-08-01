﻿namespace SnipEx.Services.Data.Models
{
    using System.Globalization;

    using MediatR;
    using Microsoft.EntityFrameworkCore;

    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Comment;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;
    using SnipEx.Services.Mediator.Comments.ReplyAdded;
    using SnipEx.Services.Mediator.Comments.CommentAdded;

    using static Common.EntityValidationConstants.Comment;

    public class CommentService(
        IRepository<Comment, Guid> commentRepository,
        IRepository<Post, Guid> postRepository,
        IMediator mediator) : ICommentService
    {
        public async Task<bool> AddCommentAsync(AddPostCommentFormModel model, string userId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(model.PostId, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return false;
            }

            var userGuid = Guid.Parse(userId);

            var comment = new Comment();
            AutoMapperConfig.MapperInstance.Map(model, comment);
            comment.PostId = postGuid;
            comment.CreatedAt = DateTime.UtcNow;
            comment.UserId = userGuid;

            await commentRepository.AddAsync(comment);
            await commentRepository.SaveChangesAsync();

            var commentAddedEvent = new CommentAddedEvent(comment.Id, comment.PostId, userGuid);
            await mediator.Publish(commentAddedEvent);

            return true;
        }

        public async Task<bool> AddReplyAsync(AddCommentReplyFormModel model, string userId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(model.PostId, out Guid postGuid);
            var isParentCommentGuidValid = ValidationUtils
                .TryGetGuid(model.ParentCommentId, out Guid parentCommentGuid);
            if (!isPostGuidValid || !isParentCommentGuidValid)
            {
                return false;
            }

            var parentComment = await commentRepository.GetByIdAsync(parentCommentGuid);
            if (parentComment == null)
            {
                return false;
            }

            Guid? referenceCommentGuid = null;
            if (!string.IsNullOrEmpty(model.ReferenceCommentId))
            {
                var isReferenceCommentGuidValid = ValidationUtils
                    .TryGetGuid(model.ReferenceCommentId, out Guid refCommentGuid);
                if (!isReferenceCommentGuidValid)
                {
                    return false;
                }

                var referenceComment = await commentRepository.GetByIdAsync(refCommentGuid);
                if (referenceComment == null)
                {
                    return false;
                }

                referenceCommentGuid = refCommentGuid;
            }

            var userGuid = Guid.Parse(userId);

            var reply = new Comment();
            AutoMapperConfig.MapperInstance.Map(model, reply);

            reply.PostId = postGuid;
            reply.CreatedAt = DateTime.UtcNow;
            reply.UserId = userGuid;
            reply.ParentCommentId = parentCommentGuid;

            await commentRepository.AddAsync(reply);
            await commentRepository.SaveChangesAsync();

            bool isMention = referenceCommentGuid.HasValue;

            var replyAddedEvent = new ReplyAddedEvent(reply.Id, parentCommentGuid, userGuid, isMention);
            await mediator.Publish(replyAddedEvent);

            return true;
        }

        public async Task<IEnumerable<CommentViewModel>> GetStructuredComments(IEnumerable<CommentViewModel> comments)
        {
            var structuredComments = new List<CommentViewModel>();

            foreach (var comment in comments)
            {
                //If no parent comment - add to top-level comments
                if (comment.ParentCommentId == null)
                {
                    structuredComments.Add(comment);
                }
            }

            return structuredComments;
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsByPostIdAsync(Guid postGuid)
        {
            var comments = await commentRepository.GetAllAttached()
                .Where(c => c.PostId == postGuid)
                .ToListAsync();

            //Materialise the query before mapping
            var filteredComments = comments
                .AsQueryable()
                .To<CommentViewModel>()
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return filteredComments;
        }

        public void SetUserLikeStatus(CommentViewModel comment, ICollection<Comment> postComments, string userId)
        {
            var userGuid = Guid.Parse(userId);

            var isCommentGuidValid = ValidationUtils.TryGetGuid(comment.Id, out Guid commentGuid);
            if (isCommentGuidValid)
            {
                var postComment = postComments.FirstOrDefault(c => c.Id == commentGuid);
                if (postComment != null)
                {
                    comment.IsLikedByCurrentUser = postComment.Likes.Any(l => l.UserId == userGuid);
                }
            }

            foreach (var reply in comment.Replies)
            {
                SetUserLikeStatus(reply, postComments, userId);
            }
        }

        public async Task<int> GetReceivedCommentsCountAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);

            var userPostIds = postRepository
                .GetAllAttached()
                .Where(p => p.UserId == userGuid)
                .Select(p => p.Id)
                .ToHashSet();

            var receivedComments = await commentRepository
                .GetAllAttached()
                .CountAsync(c => 
                    c.UserId != userGuid && c.UserId != null &&
                    userPostIds.Contains(c.PostId));

            return receivedComments;
        }
    }
}
