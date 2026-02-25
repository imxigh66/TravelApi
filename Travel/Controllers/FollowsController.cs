using Application.Common.Models;
using Application.DTO.Users;
using Application.Follows.Commands;
using Application.Follows.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class FollowsController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        public FollowsController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.Parse(claim!);
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> Follow(int id)
        {
            var result = await _mediator.Send(new FollowUserCommand
            {
                FollowerId = GetCurrentUserId(),
                FollowingId = id
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Followed successfully"));
        }

        [HttpDelete("{id}/unfollow")]
        public async Task<IActionResult> Unfollow(int id)
        {
            var result = await _mediator.Send(new UnfollowUserCommand
            {
                FollowerId = GetCurrentUserId(),
                FollowingId = id
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return NoContent();
        }


        [HttpGet("{id}/followers")]
        public async Task<ActionResult<PaginatedList<UserFollowDto>>> GetFollowers(
           int id,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetFollowersQuery
            {
                UserId = id,
                CurrentUserId = GetCurrentUserId(),
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(result);
        }

        [HttpGet("{id}/following")]
        public async Task<ActionResult<PaginatedList<UserFollowDto>>> GetFollowing(
            int id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetFollowingQuery
            {
                UserId = id,
                CurrentUserId = GetCurrentUserId(),
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(result);
        }

        [HttpGet("{id}/is-following")]
        public async Task<IActionResult> IsFollowing(int id)
        {
            var result = await _mediator.Send(new IsFollowingQuery
            {
                FollowerId = GetCurrentUserId(),
                FollowingId = id
            });

            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }
    }
}
