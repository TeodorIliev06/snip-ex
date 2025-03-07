namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Comment;

    public interface ICommentService
    {
        Task<bool> AddCommentAsync(AddCommentFormModel model, string userId);
    }
}
