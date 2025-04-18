﻿namespace SnipEx.Services.Data.Contracts
{
    public interface ILikeService
    {
        Task<bool> TogglePostLikeAsync(Guid postGuid, string userId);

        Task<bool> IsPostLikedByUserAsync(Guid postGuid, string userId);

        Task<int> GetPostLikesCountAsync(Guid postGuid);

        Task<bool> ToggleCommentLikeAsync(Guid commentGuid, string userId);

        Task<bool> IsCommentLikedByUserAsync(Guid commentGuid, string userId);

        Task<int> GetCommentLikesCountAsync(Guid commentGuid);

        Task<bool> TogglePostSaveAsync(Guid postGuid, string userId);

        Task<bool> IsPostSavedByUserAsync(Guid postGuid, string userId);
    }
}
