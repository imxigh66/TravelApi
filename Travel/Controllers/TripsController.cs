using Application.Common.Models;
using Application.DTO.Posts;
using Application.DTO.Trips;
using Application.Trips.Commands;
using Application.Trips.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripsController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        public TripsController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TripDto>>> CreateTrip([FromBody] CreateTripRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new CreateTripCommand
            {
                OwnerId = userId,
                Title = dto.Title,
                Description = dto.Description,
                TripDate = dto.TripDate,
                CountryCode = dto.CountryCode,
                City = dto.City,
                IsPublic = dto.IsPublic,
                Status = dto.Status
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<TripDto>.SuccessResponse(result.Data!));
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<List<TripDto>>>> GetMyTrips()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var query = new GetMyTripsQuery { OwnerId = userId };
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<List<TripDto>>.SuccessResponse(result));
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TripDetailDto>>> GetTripById(int id)
        {
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int uid))
                currentUserId = uid;

            var query = new GetTripByIdQuery { TripId = id, CurrentUserId = currentUserId };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return Ok(ApiResponse<TripDetailDto>.SuccessResponse(result.Data!));
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<TripDto>>>> GetUserTrips(int userId)
        {
            var query = new GetUserTripsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<TripDto>>.SuccessResponse(result));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new DeleteTripCommand { TripId = id, UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return result.Error == "Access denied."
                    ? Forbid()
                    : NotFound(ErrorResponse.NotFound(result.Error!));

            return NoContent();
        }


        [Authorize]
        [HttpPost("{tripId}/places")]
        public async Task<ActionResult<ApiResponse<TripPlaceDto>>> AddPlace(int tripId, [FromBody] AddPlaceRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new AddPlaceToTripCommand
            {
                TripId = tripId,
                UserId = userId,
                PlaceId = dto.PlaceId,
                Notes = dto.Notes
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<TripPlaceDto>.SuccessResponse(result.Data!));
        }

        [Authorize]
        [HttpDelete("{tripId}/places/{placeId}")]
        public async Task<IActionResult> RemovePlace(int tripId, int placeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new RemovePlaceFromTripCommand
            {
                TripId = tripId,
                PlaceId = placeId,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return NoContent();
        }

        [Authorize]
        [HttpPut("{tripId}/places/reorder")]
        public async Task<IActionResult> ReorderPlaces(int tripId, [FromBody] List<TripPlaceOrderItem> places)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst("sub")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new ReorderTripPlacesCommand
            {
                TripId = tripId,
                UserId = userId,
                Places = places
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return NoContent();
        }
    }
}
