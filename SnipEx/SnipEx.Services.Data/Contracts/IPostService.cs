﻿namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Post;

    public interface IPostService
    {
        Task<PostIndexViewModel> GetPostsAsync(string? tag, string? search, string? sort);

        Task<bool> AddPostAsync(AddPostFormModel model, string userId);
    }
}
