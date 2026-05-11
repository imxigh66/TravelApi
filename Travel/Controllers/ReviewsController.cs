using Application.Common.Models;
using Application.DTO.Reviews;
using Application.Reviews.Commands;
using Application.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/places/{placeId:int}/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ReviewDto>>> GetReviews(
            int placeId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPlaceReviewsQuery
            {
                PlaceId = placeId,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });

            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview(
            int placeId,
            [FromBody] CreateReviewRequest dto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var result = await _mediator.Send(new CreateReviewCommand
            {
                PlaceId = placeId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(GetReviews),
                new { placeId },
                ApiResponse<ReviewDto>.SuccessResponse(result.Data!, "Review added successfully."));
        }


        [Authorize]
        [HttpDelete("{reviewId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteReview(int placeId, int reviewId)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token"));

            var result = await _mediator.Send(new DeleteReviewCommand
            {
                ReviewId = reviewId,
                UserId = userId,
            });

            if (!result.IsSuccess)
                return result.Error == "Access denied."
                    ? Forbid()
                    : BadRequest(ErrorResponse.BadRequest(result.Error!));

            return NoContent();
        }


        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out userId);
        }

        public record CreateReviewRequest(int Rating, string? Comment);
    }
}
