using Application.CategoryTags.Commands;
using Application.CategoryTags.Queries;
using Application.Common.Models;
using Application.DTO.CategoryTags;
using Application.DTO.Places;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/category-tags")]
    public class CategoryTagsController:ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryTagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryTagCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(nameof(GetAll), ApiResponse<CategoryTagDto>.SuccessResponse(result.Data!));
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryTagDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllCategoryTagsQuery());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryTagCommand command)
        {
            command.CategoryTagId = id;
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return Ok(ApiResponse<CategoryTagDto>.SuccessResponse(result.Data!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryTagCommand { CategoryTagId = id });
            if (!result.IsSuccess)
                return NotFound(ErrorResponse.NotFound(result.Error!));

            return NoContent();
        }

        [HttpPut("places/{placeId}")]
        public async Task<IActionResult> AssignTagsToPlace(int placeId, [FromBody] List<int> categoryTagIds)
        {
            var command = new AssignTagsToPlaceCommand
            {
                PlaceId = placeId,
                CategoryTagIds = categoryTagIds
            };

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(new { message = "Tags assigned successfully" });
        }

        [HttpGet("{id}/places")]
        public async Task<ActionResult<PaginatedList<PlaceDto>>> GetPlacesByCategory(
    int id,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetPlacesByCategoryTagQuery
            {
                CategoryTagId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
