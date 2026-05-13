using Application.Common.Models;
using Application.DTO.Trips;
using Application.TripChat.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId:int}/chat")]
    [Authorize]
    public class TripChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpGet]
        public async Task<ActionResult<PaginatedList<TripMessageDto>>> GetMessages(
            int tripId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetTripMessagesQuery
            {
                TripId = tripId,
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });

            return Ok(result);
        }

        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out userId);
        }
    }
}
