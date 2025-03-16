namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Data.Models;
    using SnipEx.Web.ViewModels.Comment;

    public interface ICommentService
    {
        Task<bool> AddCommentAsync(AddPostCommentFormModel model, string userId);

        Task<bool> AddReplyAsync(AddCommentReplyFormModel model, string userId);

        Task<IEnumerable<CommentViewModel>> GetStructuredComments(IEnumerable<CommentViewModel> comments);

        Task<IEnumerable<CommentViewModel>> GetCommentsByPostIdAsync(Guid postGuid);

        void SetUserLikeStatus(CommentViewModel comment, ICollection<Comment> postComments, Guid userGuid);
    }
}
