namespace SnipEx.WebApi.Controllers
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts.Utils;
    using System;

    [Authorize]
    public class ProfilePictureApiController(
        IProfilePictureService profilePictureService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);
            string relativePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                ? "images/profile_pics/default_user.png"
                : user.ProfilePicturePath;

            return File(relativePath, profilePictureService.GetContentType(relativePath));
        }

        [HttpPost("upload")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded." });
                }

                var result = await profilePictureService.UploadProfilePictureAsync(user, file);

                if (result)
                {
                    return Ok(new { message = "Profile picture uploaded successfully" });
                }

                return BadRequest(new { message = "Failed to upload profile picture" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);
            var result = await profilePictureService.RemoveProfilePictureAsync(user);

            return result
                ? Ok(new { message = "Profile picture removed successfully" })
                : BadRequest(new { message = "Failed to remove profile picture" });
        }
    }
}
