namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Post;
    using SnipEx.Web.ViewModels.User;

    public interface IUserService
    {
        Task<ProfileInformationViewModel> GetProfileInformationAsync(string userId);

        Task<BookmarkViewModel> GetUserBookmarksAsync(string userId);

        Task<IEnumerable<PostCardViewModel>> GetUserSnippetsAsync(string userId);

        Task<IEnumerable<ConnectionViewModel>> GetUserConnectionsAsync(string userId,
            int skip = 0, int take = 4);

        Task<IEnumerable<ConnectionViewModel>> GetUserMutualConnectionsAsync(string userId,
            int skip = 0, int take = 4);

        Task<Dictionary<string, int>> GetTotalLikesReceivedByUserAsync(List<string> userIds);
    }
}
