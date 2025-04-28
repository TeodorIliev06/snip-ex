namespace SnipEx.WebApi.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Common;
    using SnipEx.Services.Data.Contracts;

    using static SnipEx.Common.PopUpMessages;
    using static SnipEx.Web.ViewModels.DTOs.ApiResponses;

    [Authorize]
    public class UserActionApiController(
        IUserActionService userActionService) : BaseApiController
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

            var isLiked = await userActionService.TogglePostLikeAsync(postGuid, userId);
            var likesCount = await userActionService.GetPostLikesCountAsync(postGuid);

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

            var isSaved = await userActionService.TogglePostSaveAsync(postGuid, userId);
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

            var isLiked = await userActionService.ToggleCommentLikeAsync(commentGuid, userId);
            var likesCount = await userActionService.GetCommentLikesCountAsync(commentGuid);

            return Ok(new ToggleCommentLikeResponse()
            {
                IsLiked = isLiked,
                LikesCount = likesCount
            });
        }

