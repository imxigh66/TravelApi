using Application.Comments.Commands;
using Application.Comments.Queries;
using Application.Common.Models;
using Application.DTO.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TravelApi.Controllers
{

    [ApiController]
    [Route("api/posts")]
    public class CommentsController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        public CommentsController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("{postId}/comments")]  
        public async Task<ActionResult<ApiResponse<CommentDto>>> CreateComment(
       int postId,  
       [FromBody] CreateCommentRequest dto)  
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new CreateCommentCommand
            {
                UserId = userId,
                PostId = postId, 
                Content = dto.Content
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<CommentDto>.SuccessResponse(result.Data!));
        }


        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<PaginatedList<CommentDto>>> GetComments(
    int postId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetCommentsQuery
            {
                PostId = postId,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var result = await _mediator.Send(new DeleteCommentCommand
            {
                CommentId = commentId,
                UserId = userId  // хэндлер проверит что comment.UserId == userId
            });

            if (!result)
                return NotFound(ErrorResponse.NotFound("Comment not found or you are not the author"));

            return NoContent();
        }
    }
}
