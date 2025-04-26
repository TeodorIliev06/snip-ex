namespace SnipEx.WebApi.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Common;
    using SnipEx.Web.ViewModels.Comment;
    using SnipEx.Services.Data.Contracts;

    using static SnipEx.Common.PopUpMessages;

    [Authorize]
    public class CommentApiController(
        ICommentService commentService) : BaseApiController
    {
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddComment([FromBody] AddPostCommentFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.InvalidCommentLength));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool isAdded = await commentService.AddCommentAsync(model, userId);
            if (!isAdded)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.InvalidCommentOperation));
            }

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddReply([FromBody] AddCommentReplyFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.InvalidCommentLength));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isReplyAdded = await commentService.AddReplyAsync(model, userId);

            if (!isReplyAdded)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.InvalidCommentOperation));
            }

            return Ok();
        }

        [HttpGet("[action]/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PopUpError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComments(string postId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(postId, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return BadRequest(ApiResponse.Fail(PopUpError.NonExistingPost));
            }

            var comments = await commentService.GetCommentsByPostIdAsync(postGuid);
            return Ok(comments);
        }
    }
}
