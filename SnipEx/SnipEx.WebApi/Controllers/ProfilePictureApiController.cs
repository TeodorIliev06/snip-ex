namespace SnipEx.WebApi.Controllers
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts.Utils;
    using SnipEx.Web.ViewModels.DTOs;

    [Authorize]
    public class ProfilePictureApiController(
        IProfilePictureService profilePictureService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment) : BaseApiController
    {
        [HttpGet("GetProfilePicture")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);

            return PhysicalFile(Path.Combine(environment.WebRootPath, user.ProfilePicturePath),
                profilePictureService.GetContentType(user.ProfilePicturePath));
        }

        /*TODO:
            Endpoint is working as intended
            Swagger crashes when uploading
            Think of a workaround
        */
        [HttpPost("UploadProfilePicture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadProfilePicture([FromForm] FileUploadRequestDto request)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                if (request == null || request.File.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded." });
                }

                var result = await profilePictureService
                    .UploadProfilePictureAsync(user, request.File);

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

        [HttpDelete("DeleteProfilePicture")]
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
