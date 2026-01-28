using Application.Auth.Register.Commands;
using Application.Places.Commands;
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
        public async Task<IActionResult> GetAllPlaces()
        {
            var query = new Application.Places.Queries.GetAllPlacesQuery();
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
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
