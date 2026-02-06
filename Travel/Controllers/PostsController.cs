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
        public async Task<ActionResult<ApiResponse<PostDto>>> CreatePost(CreatePostCommand command)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirst("sub");

            if (userIdClaim == null)
            {
                // ДОБАВЬ ЛОГИРОВАНИЕ ДЛЯ ОТЛАДКИ
                var allClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
                Console.WriteLine($"All claims: {allClaims}"); // Посмотри что в токене
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID in token"));


            command.UserId = userId;


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


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PaginatedList<PostDto>>> GetAllPosts(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPostsQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
