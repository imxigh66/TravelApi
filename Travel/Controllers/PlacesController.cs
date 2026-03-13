using Application.Auth.Register.Commands;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Places.Commands;
using Application.Places.Queries;
using Application.Posts.Commands;
using Application.Posts.Queries;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/places")]
    public class PlacesController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        public PlacesController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> CreatePlace([FromForm] CreatePlaceRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

            PlaceCategory? category = null;
            if (!string.IsNullOrEmpty(request.Category))
            {
                if (!Enum.TryParse<PlaceCategory>(request.Category, true, out var c))
                    return BadRequest(ErrorResponse.BadRequest($"Invalid category: '{request.Category}'"));
                category = c;
            }

            PlaceType? placeType = null;
            if (!string.IsNullOrEmpty(request.PlaceType))
            {
                if (!Enum.TryParse<PlaceType>(request.PlaceType, true, out var pt))
                    return BadRequest(ErrorResponse.BadRequest($"Invalid placeType: '{request.PlaceType}'"));
                placeType = pt;
            }

            List<MoodType>? moods = null;
            if (request.Moods != null)
            {
                moods = new List<MoodType>();
                foreach (var moodStr in request.Moods)
                {
                    foreach (var part in moodStr.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (Enum.TryParse<MoodType>(part.Trim(), true, out var mood))
                            moods.Add(mood);
                    }
                }
            }

            var command = new CreatePlaceCommand
            {
                Name = request.Name,
                Description = request.Description,
                CountryCode = request.CountryCode,
                City = request.City,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Category = category ?? PlaceCategory.Food,
                PlaceType = placeType ?? PlaceType.Restaurant,
                AdditionalInfoJson = request.AdditionalInfoJson,
                Moods = moods,
                Images = request.Images?.ToList(),
                CreatedBy = userId,
                CreatedByRole = role,
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(GetPlaceById),
                new { id = result.Data!.PlaceId },
                ApiResponse<PlaceDto>.SuccessResponse(result.Data,
                    role == "Moderator"
                        ? "Place created and published successfully"
                        : "Place submitted for moderation. It will be visible after review."));
        }



        [HttpGet("pending")]
        [Authorize(Roles = "Moderator")]
        [ProducesResponseType(typeof(PaginatedList<PlaceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<PlaceDto>>> GetPendingPlaces(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetPendingPlacesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Moderator")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> ApprovePlace(int id)
        {
            var modId = GetCurrentUserId();

            var result = await _mediator.Send(new ApproveOrRejectPlaceCommand
            {
                PlaceId = id,
                Approve = true,
                ModeratorId = modId
            });

            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return Ok(ApiResponse<PlaceDto>.SuccessResponse(result.Data!, "Place approved successfully"));
        }


        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Moderator")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> RejectPlace(
           int id,
           [FromBody] RejectPlaceRequest body)
        {
            var modId = GetCurrentUserId();

            var result = await _mediator.Send(new ApproveOrRejectPlaceCommand
            {
                PlaceId = id,
                Approve = false,
                RejectionReason = body.Reason,
                ModeratorId = modId
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<PlaceDto>.SuccessResponse(result.Data!, "Place rejected"));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> GetPlaceById(int id)
        {
            var query = new GetPlaceByIdQuery { PlaceId = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return Ok(ApiResponse<PlaceDto>.SuccessResponse(result.Data!));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<PlaceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<PlaceDto>>> GetAllPlaces(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] int? categoryTagId = null,  // подборка модератора
     [FromQuery] string? category = null,   // PlaceCategory
     [FromQuery] string? placeType = null,   // PlaceType
     [FromQuery] string? mood = null,   // MoodType
     [FromQuery] string? city = null,
     [FromQuery] string? countryCode = null,
     [FromQuery] string? sortBy = null,
     [FromQuery] string? search = null)   // rating | popular | newest
        {
            PlaceCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category))
            {
                if (!Enum.TryParse<PlaceCategory>(category, true, out var c))
                    return BadRequest(ErrorResponse.BadRequest($"Invalid category: '{category}'"));
                categoryEnum = c;
            }

            PlaceType? placeTypeEnum = null;
            if (!string.IsNullOrEmpty(placeType))
            {
                if (!Enum.TryParse<PlaceType>(placeType, true, out var pt))
                    return BadRequest(ErrorResponse.BadRequest($"Invalid placeType: '{placeType}'"));
                placeTypeEnum = pt;
            }

            MoodType? moodEnum = null;
            if (!string.IsNullOrEmpty(mood))
            {
                if (!Enum.TryParse<MoodType>(mood, true, out var m))
                    return BadRequest(ErrorResponse.BadRequest($"Invalid mood: '{mood}'"));
                moodEnum = m;
            }

            var query = new GetAllPlacesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CategoryTagId = categoryTagId,
                Category = categoryEnum,
                PlaceType = placeTypeEnum,
                Mood = moodEnum,
                City = city,
                CountryCode = countryCode,
                SortBy = sortBy,
                Search = search
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Moderator")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> UpdatePlace(
            int id,
            [FromForm] UpdatePlaceRequest request)
        {
            PlaceCategory? category = null;
            if (!string.IsNullOrEmpty(request.Category))
            {
                if (!Enum.TryParse<PlaceCategory>(request.Category, true, out var c))
                    return BadRequest(ErrorResponse.BadRequest("Invalid category"));
                category = c;
            }

            PlaceType? placeType = null;
            if (!string.IsNullOrEmpty(request.PlaceType))
            {
                if (!Enum.TryParse<PlaceType>(request.PlaceType, true, out var pt))
                    return BadRequest(ErrorResponse.BadRequest("Invalid place type"));
                placeType = pt;
            }

            List<MoodType>? moods = null;
            if (request.Moods != null)
            {
                moods = new List<MoodType>();
                foreach (var moodStr in request.Moods)
                {
                    foreach (var part in moodStr.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (Enum.TryParse<MoodType>(part.Trim(), true, out var mood))
                            moods.Add(mood);
                    }
                }
            }

            var command = new UpdatePlaceCommand
            {
                PlaceId = id,
                Name = request.Name,
                Description = request.Description,
                CountryCode = request.CountryCode,
                City = request.City,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Category = category,
                PlaceType = placeType,
                AdditionalInfoJson = request.AdditionalInfoJson,
                Moods = moods,
                NewImages = request.NewImages?.ToList(),
                DeleteImageIds = request.DeleteImageIds,
                CoverImageId = request.CoverImageId,
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return Ok(ApiResponse<PlaceDto>.SuccessResponse(result.Data!, "Place updated successfully"));
        }


        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPostsByPlace(
    int id,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsByPlaceQuery
            {
                PlaceId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }



        [HttpGet("{id}/nearby")]
        public async Task<IActionResult> GetNearby(int id)
        {
            var result = await _mediator.Send(new GetNearbyQuery { PlaceId = id });
            if (!result.IsSuccess) return NotFound(result.Data);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("{id}/save")]
        public async Task<IActionResult> Save(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new SavePlaceCommand
            {
                UserId = userId,
                PlaceId = id
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<object>.SuccessResponse("Place saved successfully"));
        }


        [HttpDelete("{id}/unsave")]
        public async Task<IActionResult> Unsave(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var command = new UnsavePlaceCommand
            {
                UserId = userId,
                PlaceId = id
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return NoContent(); 
        }

       


        [Authorize]
        [HttpGet("{id}/is-saved")]
        public async Task<IActionResult> IsSaved(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var query = new IsSavedQuery
            {
                UserId = userId,
                PlaceId = id
            };
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<bool>.SuccessResponse(result, "IsSaved retrieved successfully"));
        }


        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.Parse(claim!);
        }
    }

}
