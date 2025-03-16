namespace SnipEx.Services.Data
{
    using System.Globalization;
    using Microsoft.EntityFrameworkCore;
    using SnipEx.Data.Models;
    using SnipEx.Services.Mapping;
    using SnipEx.Web.ViewModels.Comment;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Data.Repositories.Contracts;

    using static Common.EntityValidationConstants.Comment;
    using SnipEx.Common;
    using SnipEx.Web.ViewModels.Post;

    public class CommentService(
        IRepository<Comment, Guid> commentRepository) : ICommentService
    {
        public async Task<bool> AddCommentAsync(AddPostCommentFormModel model, string userId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(model.PostId, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return false;
            }

            bool isCreationDateValid = DateTime
                .TryParseExact(model.CreatedAt, CreatedAtFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate);
            if (!isCreationDateValid)
            {
                return false;
            }

            var userGuid = Guid.Parse(userId);

            var comment = new Comment();
            AutoMapperConfig.MapperInstance.Map(model, comment);
            comment.PostId = postGuid;
            comment.CreatedAt = creationDate;
            comment.UserId = userGuid;

            await commentRepository.AddAsync(comment);
            await commentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddReplyAsync(AddCommentReplyFormModel model, string userId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(model.PostId, out Guid postGuid);
            var isParentCommentGuidValid = ValidationUtils.TryGetGuid(model.ParentCommentId, out Guid parentCommentGuid);
            if (!isPostGuidValid || !isParentCommentGuidValid)
            {
                return false;
            }

            bool isCreationDateValid = DateTime
                .TryParseExact(model.CreatedAt, CreatedAtFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate);

            if (!isCreationDateValid)
            {
                return false;
            }

            var userGuid = Guid.Parse(userId);

            var reply = new Comment();
            AutoMapperConfig.MapperInstance.Map(model, reply);
            reply.PostId = postGuid;
            reply.CreatedAt = creationDate;
            reply.UserId = userGuid;
            reply.ParentCommentId = parentCommentGuid;

            await commentRepository.AddAsync(reply);
            await commentRepository.SaveChangesAsync();

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
            //TODO: Fix comments not mapping
            var comments = await commentRepository.GetAllAttached()
                .Include(p => p.Post)
                .Where(c => c.PostId == postGuid && c.ParentCommentId == null)
                .OrderByDescending(p => p.CreatedAt)
                .To<CommentViewModel>()
                .ToListAsync();

            return comments;
        }

        public void SetUserLikeStatus(CommentViewModel comment, ICollection<Comment> postComments, Guid userGuid)
        {
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
                SetUserLikeStatus(reply, postComments, userGuid);
            }
        }
    }
}
