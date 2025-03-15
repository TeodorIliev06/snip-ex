namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Comment;

    public interface ICommentService
    {
        Task<bool> AddCommentAsync(AddPostCommentFormModel model, string userId);

        Task<IEnumerable<CommentViewModel>> GetStructuredComments(IEnumerable<CommentViewModel> comments);
    }
}
