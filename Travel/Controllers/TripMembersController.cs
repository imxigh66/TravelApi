using Application.Common.Models;
using Application.DTO.Trips;
using Application.TripMembers.Commands;
using Application.TripMembers.Queries;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId:int}/members")]
    [Authorize]
    public class TripMembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripMembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/trips/{tripId}/members
        [HttpGet]
        public async Task<ActionResult<List<TripMemberDto>>> GetMembers(int tripId)
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var result = await _mediator.Send(new GetTripMembersQuery
            {
                TripId = tripId,
                RequesterId = userId,
            });

            return Ok(result);
        }

        // POST /api/trips/{tripId}/members
        // body: { targetUserId: 5, role: "Editor" }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TripMemberDto>>> AddMember(
            int tripId, [FromBody] AddMemberRequest dto)
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var result = await _mediator.Send(new AddTripMemberCommand
            {
                TripId = tripId,
                OwnerId = userId,
                TargetUserId = dto.TargetUserId,
                Role = dto.Role,
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<TripMemberDto>.SuccessResponse(result.Data!));
        }

        // PATCH /api/trips/{tripId}/members/{targetUserId}
        // body: { role: "Viewer" }
        [HttpPatch("{targetUserId:int}")]
        public async Task<ActionResult<ApiResponse<TripMemberDto>>> UpdateRole(
            int tripId, int targetUserId, [FromBody] UpdateRoleRequest dto)
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var result = await _mediator.Send(new UpdateTripMemberRoleCommand
            {
                TripId = tripId,
                OwnerId = userId,
                TargetUserId = targetUserId,
                NewRole = dto.Role,
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<TripMemberDto>.SuccessResponse(result.Data!));
        }

        // DELETE /api/trips/{tripId}/members/{targetUserId}
        [HttpDelete("{targetUserId:int}")]
        public async Task<IActionResult> RemoveMember(int tripId, int targetUserId)
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var result = await _mediator.Send(new RemoveTripMemberCommand
            {
                TripId = tripId,
                RequesterId = userId,
                TargetUserId = targetUserId,
            });

            if (!result.IsSuccess)
                return result.Error!.Contains("denied")
                    ? Forbid()
                    : BadRequest(ErrorResponse.BadRequest(result.Error));

            return NoContent();
        }

        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out userId);
        }
    }

    public record AddMemberRequest(int TargetUserId, TripMemberRole Role);
    public record UpdateRoleRequest(TripMemberRole Role);
}
