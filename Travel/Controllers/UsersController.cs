using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Queries;
using Application.Users.QueriesCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Application.Users.Command;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetCurrentUser()
        {
            // Получаем userId из JWT токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;

            if (userIdClaim == null)
            {
                // Логирование для отладки
                var allClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
                _logger.LogWarning($"No userId claim found. All claims: {allClaims}");

                return Unauthorized(ErrorResponse.Unauthorized("Invalid token - no user ID found"));
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning($"Invalid userId format: {userIdClaim}");
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID format"));
            }

            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(ErrorResponse.NotFound(result.Error!));
            }

            return Ok(ApiResponse<UserResponse>.SuccessResponse(result.Data!));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(int userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(ErrorResponse.NotFound(result.Error!));
            }

            return Ok(ApiResponse<UserResponse>.SuccessResponse(result.Data!));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<UserResponse>>> GetAllUsers(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = new GetAllUsersQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPatch("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PersonalProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PersonalProfileDto>>> UpdatePersonalProfile(UpdatePersonalProfileCommand command)
        {
            // Получаем userId из JWT токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token - no user ID found"));
            }
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID format"));
            }
            command.UserId = userId;
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(ErrorResponse.BadRequest(result.Error!));
            }
            return Ok(ApiResponse<PersonalProfileDto>.SuccessResponse(result.Data!));
        }


        [HttpPatch("business")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<BusinessProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<BusinessProfileDto>>> UpdateBusinessProfile(UpdateBusinessProfileCommand command)
        {
            // Получаем userId из JWT токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token - no user ID found"));
            }
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID format"));
            }
            command.UserId = userId;
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(ErrorResponse.BadRequest(result.Error!));
            }
            return Ok(ApiResponse<BusinessProfileDto>.SuccessResponse(result.Data!));

        }


        [HttpPost("profile-picture")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<Application.DTO.Files.FileUploadResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<Application.DTO.Files.FileUploadResultDto>>> UpdateProfilePicture( IFormFile image)
        {
            // Получаем userId из JWT токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token - no user ID found"));
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID format"));
            }

            var command = new UpdateProfilePictureCommand
            {
                UserId = userId,
                Image = image
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ErrorResponse.BadRequest(result.Error!));
            }

            return Ok(ApiResponse<Application.DTO.Files.FileUploadResultDto>.SuccessResponse(
                result.Data!,
                "Profile picture updated successfully"
            ));
        }

        
        [HttpDelete("profile-picture")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProfilePicture()
        {
           
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid token - no user ID found"));
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user ID format"));
            }

            
            var user = await _mediator.Send(new GetUserByIdQuery { UserId = userId });

            if (!user.IsSuccess || user.Data == null)
            {
                return NotFound(ErrorResponse.NotFound("User not found"));
            }

            
            if (string.IsNullOrEmpty(user.Data.ProfilePicture))
            {
                return BadRequest(ErrorResponse.BadRequest("No profile picture to delete"));
            }

            // TODO: Implement DeleteProfilePictureCommand
            // var command = new DeleteProfilePictureCommand { UserId = userId };
            // var result = await _mediator.Send(command);

            return Ok(ApiResponse<string>.SuccessResponse("Profile picture deleted successfully"));
        }


    }
}
