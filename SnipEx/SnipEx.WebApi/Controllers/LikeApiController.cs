using Microsoft.AspNetCore.Mvc;

namespace SnipEx.WebApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts;

    public class LikeApiController(
        ILikeService likeService,
        UserManager<ApplicationUser> userManager) : BaseApiController
    {
        /*TODO: 
            1. introduce authorization via jwt
            2. introduce SignalR for real-time effect
        */
        //[Authorize]
        [HttpPost("[action]/{postId}")]
        public async Task<IActionResult> TogglePostLike(string postId, [FromBody] ToggleLikeRequest request)
        {
            var isGuidValid = ValidationUtils.TryGetGuid(postId, out Guid postGuid);
            if (!isGuidValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                return Unauthorized();
            }

            var isLiked = await likeService.TogglePostLikeAsync(postGuid, request.UserId);
            var likesCount = await likeService.GetPostLikesCountAsync(postGuid);

            return Ok(new { isLiked, likesCount });
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestRoute()
        {
            return Ok("Route works!");
        }

        // DTO class
        public class ToggleLikeRequest
        {
            public string UserId { get; set; }
        }
    }
}
