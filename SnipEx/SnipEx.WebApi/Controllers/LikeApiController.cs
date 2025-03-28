﻿using Microsoft.AspNetCore.Mvc;

namespace SnipEx.WebApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts;

    [Authorize]
    public class LikeApiController(
        ILikeService likeService,
        UserManager<ApplicationUser> userManager) : BaseApiController
    {
        /*TODO: introduce SignalR for real-time effect*/
        
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

        [HttpPost("[action]/{commentId}")]
        public async Task<IActionResult> ToggleCommentLike(string commentId, [FromBody] ToggleLikeRequest request)
        {
            var isGuidValid = ValidationUtils.TryGetGuid(commentId, out Guid commentGuid);
            if (!isGuidValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                return Unauthorized();
            }

            var isLiked = await likeService.ToggleCommentLikeAsync(commentGuid, request.UserId);
            var likesCount = await likeService.GetCommentLikesCountAsync(commentGuid);

            return Ok(new { isLiked, likesCount });
        }

        // DTO class
        public class ToggleLikeRequest
        {
            public string UserId { get; set; }
        }
    }
}
