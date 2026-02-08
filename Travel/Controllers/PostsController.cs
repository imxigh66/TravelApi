using Application.Common.Models;
using Application.DTO.Posts;
using Application.DTO.Users;
using Application.Posts.Commands;
using Application.Posts.Queries;
using Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController:ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        public PostsController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize] 
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PostDto>>> CreatePost(
    [FromForm] CreatePostWithImageDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new CreatePostCommand
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content,
                PlaceId = dto.PlaceId,
                Images = dto.Images  // 👈 Массив файлов
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(GetPostById),
                new { id = result.Data!.PostId },
                ApiResponse<PostDto>.SuccessResponse(
                    result.Data,
                    "Post created successfully"
                )
            );
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var result = await _mediator.Send(new GetPostByIdQuery { PostId = id });
            if (!result.IsSuccess) return NotFound(ErrorResponse.NotFound(result.Error!));
            return Ok(ApiResponse<PostDto>.SuccessResponse(result.Data!));
        }


        
        [HttpGet]
        public async Task<ActionResult<PaginatedList<PostDto>>> GetAllPosts(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPostsQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }



        [Authorize]
        [HttpGet("my-posts")]
        [ProducesResponseType(typeof(PaginatedList<PostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedList<PostDto>>> GetMyPosts(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var query = new GetAllPostsQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [Authorize]
        [HttpPost("{postId}/like")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new LikePostCommand
            {
                PostId = postId,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(ErrorResponse.NotFound("Post not found"));

            return Ok(new { message = "Post liked successfully" });
        }

        
        [Authorize]
        [HttpDelete("{postId}/like")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new UnlikePostCommand
            {
                PostId = postId,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(ErrorResponse.NotFound("Like not found"));

            return Ok(new { message = "Post unliked successfully" });
        }

        
        [HttpGet("{postId}/likes")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetPostLikes(int postId)
        {
            var query = new GetPostLikesQuery { PostId = postId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
