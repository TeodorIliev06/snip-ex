namespace SnipEx.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SnipEx.Common;
    using SnipEx.Services.Data.Contracts;
    using SnipEx.Web.ViewModels.Comment;

    public class CommentApiController(
        ICommentService commentService) : BaseApiController
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> AddReply([FromBody] AddCommentReplyFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var isReplyAdded = await commentService.AddReplyAsync(model, model.UserId!);

            if (!isReplyAdded)
            {
                //Add error
                return BadRequest(this.ModelState);
            }

            return Ok();
        }

        [HttpGet("[action]/{postId}")]
        public async Task<IActionResult> GetComments(string postId)
        {
            var isPostGuidValid = ValidationUtils.TryGetGuid(postId, out Guid postGuid);
            if (!isPostGuidValid)
            {
                return BadRequest(new { message = "Invalid Post ID." });
            }

            var comments = await commentService.GetCommentsByPostIdAsync(postGuid);
            return Ok(comments);
        }
    }
}
