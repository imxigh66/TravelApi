using Application.Common.Models;
using Application.DTO.Posts;
using Application.DTO.Users;
using Application.Posts.Queries;
using Application.Users.Queries;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Application.Posts.Commands.CreatePostCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
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
    }
}
