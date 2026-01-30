using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Queries;
using Application.Users.QueriesCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController:ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<UserDto>>> GetAllUsers(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetAllUsersQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
