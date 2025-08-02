namespace SnipEx.Services.Data.Contracts
{
    public interface IUserActionService
    {
        Task<bool> TogglePostLikeAsync(Guid postGuid, string userId);

        Task<bool> IsPostLikedByUserAsync(Guid postGuid, string userId);

        Task<int> GetPostLikesCountAsync(Guid postGuid);

        Task<bool> ToggleCommentLikeAsync(Guid commentGuid, string userId);

        Task<bool> IsCommentLikedByUserAsync(Guid commentGuid, string userId);

        Task<int> GetCommentLikesCountAsync(Guid commentGuid);

        Task<bool> TogglePostSaveAsync(Guid postGuid, string userId);

        Task<bool> IsPostSavedByUserAsync(Guid postGuid, string userId);

        Task<bool> ToggleConnectionAsync(string targetUserId, string currentUserId);

        Task<bool> DoesConnectionExistAsync(string currentUserId, string targetUserId);

        Task<int> GetConnectionsCountAsync(string targetUserId);

        Task<int> GetMutualConnectionsCountAsync(string targetUserId);

        Task<Dictionary<string, int>> GetMutualConnectionsCountByUserAsync(string currentUserId,
            List<string> targetUserIds);

        Task<bool> IncrementPostViewsAsync(Guid postGuid, string userId);
    }
}
