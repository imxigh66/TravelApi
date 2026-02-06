using Application.Auth.Register.Commands;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Places.Commands;
using Application.Places.Queries;
using Application.Posts.Queries;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreatePlace([FromBody] CreatePlaceCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<PlaceDto>>> GetAllPlaces(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPlacesQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{placeId}")]
        public async Task<IActionResult> GetPlaceById(int placeId)
        {
            var query = new Application.Places.Queries.GetPlaceByIdQuery { PlaceId = placeId };
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.Error });
            }
            return Ok(result.Data);
        }
    }

}
