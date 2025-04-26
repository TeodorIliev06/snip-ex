namespace SnipEx.WebApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SnipEx.Common;
    using SnipEx.Services.Data.Contracts;
    using System.Security.Claims;
    using static SnipEx.Common.PopUpMessages;
    using static SnipEx.Web.ViewModels.DTOs.ApiResponses;

    [Authorize]
    public class LikeApiController(
        ILikeService likeService) : BaseApiController
    {
        [HttpPost("[action]/{postId}")]
        [ProducesResponseType(typeof(TogglePostLikeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TogglePostLike(string postId)
        {
            var isGuidValid = ValidationUtils.TryGetGuid(postId, out Guid postGuid);
            if (!isGuidValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.NonExistingPost));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isLiked = await likeService.TogglePostLikeAsync(postGuid, userId);
            var likesCount = await likeService.GetPostLikesCountAsync(postGuid);

            return Ok(new TogglePostLikeResponse
            {
                IsLiked = isLiked,
                LikesCount = likesCount
            });
        }

        [HttpPost("[action]/{postId}")]
        [ProducesResponseType(typeof(TogglePostSaveResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TogglePostSave(string postId)
        {
            var isGuidValid = ValidationUtils.TryGetGuid(postId, out Guid postGuid);
            if (!isGuidValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.NonExistingPost));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isSaved = await likeService.TogglePostSaveAsync(postGuid, userId);
            return Ok(new TogglePostSaveResponse()
            {
                IsSaved = isSaved
            });
        }

        [HttpPost("[action]/{commentId}")]
        [ProducesResponseType(typeof(ToggleCommentLikeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ToggleCommentLike(string commentId)
        {
            var isGuidValid = ValidationUtils.TryGetGuid(commentId, out Guid commentGuid);
            if (!isGuidValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.NonExistingComment));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isLiked = await likeService.ToggleCommentLikeAsync(commentGuid, userId);
            var likesCount = await likeService.GetCommentLikesCountAsync(commentGuid);

            return Ok(new ToggleCommentLikeResponse()
            {
                IsLiked = isLiked,
                LikesCount = likesCount
            });
        }
    }
}
