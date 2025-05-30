namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Web.ViewModels.User;

    public interface IUserService
    {
        Task<ProfileInformationViewModel> GetProfileInformationAsync(string userId);

        Task<BookmarkViewModel> GetUserBookmarksAsync(string userId);

        Task<IEnumerable<PostCardViewModel>> GetUserSnippetsAsync(string userId);

        Task<IEnumerable<ConnectionViewModel>> GetUserConnectionsAsync(string userId);
    }
}
