namespace SnipEx.Services.Data.Models.Utils
{
    using System.IO;
    using System.Drawing;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts.Utils;

    //Move service in web app to avoid coupling Services.Data with Web project
    public class ProfilePictureService(
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment) : IProfilePictureService
    {
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024;

        //TODO: Add svg support
        private static readonly string[] ALLOWED_EXTENSIONS = new[]
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        public async Task<bool> UploadProfilePictureAsync(ApplicationUser user, IFormFile file)
        {
            ValidateProfilePicture(file);

            string uploadsFolder = Path.Combine(environment.WebRootPath, "images", "profile_pics");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{user.Id}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            using (var image = Image.FromFile(filePath))
            {
                if (image.Width != image.Height)
                {
                    File.Delete(filePath);
                    throw new ValidationException("Profile picture must be a square image");
                }

                if (image.Width > 500 || image.Height > 500)
                {
                    File.Delete(filePath);
                    throw new ValidationException("Image should be 500x500 pixels or smaller");
                }
            }

            // Store relative path for web rendering
            string relativePath = Path.Combine("images", "profile_pics", uniqueFileName);
            user.ProfilePicturePath = relativePath;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private static void ValidateProfilePicture(IFormFile file)
        {
            if (file.Length > MAX_FILE_SIZE)
            {
                throw new ValidationException("File size cannot exceed 5MB");
            }

            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ALLOWED_EXTENSIONS.Contains(ext))
            {
                throw new ValidationException("Invalid file type. Allowed types are JPEG, PNG, GIF, and WebP.");
            }
        }

        public async Task<bool> RemoveProfilePictureAsync(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.ProfilePicturePath))
                return true;

            string fullPath = Path.Combine(environment.WebRootPath, user.ProfilePicturePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            user.ProfilePicturePath = null;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}