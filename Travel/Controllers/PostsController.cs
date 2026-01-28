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
        public async Task<IActionResult> GetAllPosts()
        {
            var query = new Application.Posts.Queries.GetAllPostsQuery();
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }
    }
}
