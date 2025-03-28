namespace SnipEx.Services.Data.Contracts.Utils
{
    using Microsoft.AspNetCore.Http;

    using SnipEx.Data.Models;

    public interface IProfilePictureService
    {
        Task<bool> UploadProfilePictureAsync(ApplicationUser user, IFormFile file);

        Task<bool> RemoveProfilePictureAsync(ApplicationUser user);

        string GetContentType(string fileName);
    }
}
