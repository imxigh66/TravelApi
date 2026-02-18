using Application.Auth.Register.Commands;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Places.Commands;
using Application.Places.Queries;
using Application.Posts.Queries;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;
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
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<PlaceDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PlaceDto>>> CreatePlace([FromForm] CreatePlaceRequest request)
        {
            // Получаем userId из токена
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
            //               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
            //               ?? User.FindFirst("sub");

            //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            //{
            //    return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));
            //}

            // Парсим Category из строки в enum
            if (!Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
            {
                return BadRequest(ErrorResponse.BadRequest("Invalid category"));
            }

            // Парсим PlaceType из строки в enum
            if (!Enum.TryParse<PlaceType>(request.PlaceType, true, out var placeType))
            {
                return BadRequest(ErrorResponse.BadRequest("Invalid place type"));
            }

            var moods = new List<MoodType>();

            if (request.Moods != null)
            {
                foreach (var moodStr in request.Moods)
                {
                    // Разбиваем на случай если Swagger прислал "16,10" одной строкой
                    var parts = moodStr.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var part in parts)
                    {
                        var trimmed = part.Trim();

                        // Пробуем парсить как строку ("WithCompany")
                        if (Enum.TryParse<MoodType>(trimmed, ignoreCase: true, out var mood)
                            && Enum.IsDefined(typeof(MoodType), mood)) // ← не допускает числовые комбинации
                        {
                            moods.Add(mood);
                        }
                    }
                }

                moods = moods.Distinct().ToList();
            }
            //PlaceAdditionalInfo? additionalInfo = null;

            //if (!string.IsNullOrEmpty(request.AdditionalInfoJson))
            //{
            //    try
            //    {
            //        additionalInfo = category switch
            //        {
            //            PlaceCategory.Food => JsonSerializer.Deserialize<FoodPlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Accommodation => JsonSerializer.Deserialize<AccommodationPlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Culture => JsonSerializer.Deserialize<CulturePlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Nature => JsonSerializer.Deserialize<NaturePlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Entertainment => JsonSerializer.Deserialize<EntertainmentPlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Shopping => JsonSerializer.Deserialize<ShoppingPlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Transport => JsonSerializer.Deserialize<TransportPlaceInfo>(request.AdditionalInfoJson),
            //            PlaceCategory.Services => JsonSerializer.Deserialize<ServicePlaceInfo>(request.AdditionalInfoJson),
            //            _ => null
            //        };
            //    }
            //    catch (JsonException)
            //    {
            //        return BadRequest(ErrorResponse.BadRequest("Invalid AdditionalInfo JSON format"));
            //    }
            //}

            var command = new CreatePlaceCommand
            {
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
                Images = request.Images?.ToList()
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(GetPlaceById),
                new { id = result.Data!.PlaceId },
                ApiResponse<PlaceDto>.SuccessResponse(result.Data, "Place created successfully"));
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
    [FromQuery] string? category = null,
    [FromQuery] string? city = null,
    [FromQuery] string? mood = null)
        {
            MoodType? moodEnum = null;

            if (!string.IsNullOrEmpty(mood))
            {
                if (Enum.TryParse<MoodType>(mood, out var parsedMood))
                    moodEnum = parsedMood;
                else
                    return BadRequest($"Invalid mood value: '{mood}'");
            }

            var query = new GetAllPlacesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                City = city,
                Category = category,
                Mood = moodEnum
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}
