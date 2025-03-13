namespace SnipEx.Services.Data.Contracts
{
    public interface ILikeService
    {
        Task<bool> TogglePostLikeAsync(Guid postGuid, string userId);

        Task<bool> IsPostLikedByUserAsync(Guid postGuid, string userId);

        Task<int> GetPostLikesCountAsync(Guid postGuid);
    }
}
