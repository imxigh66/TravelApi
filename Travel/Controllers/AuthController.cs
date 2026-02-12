using Application.Auth.Login.Commands;
using Application.Auth.RefreshToken.Commands;
using Application.Auth.Register.Commands;
using Application.Common.Models;
using Application.DTO.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<RegisterDto>>> Register(RegisterCommand command)
        {

            command.BaseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(Register),
                ApiResponse<RegisterDto>.SuccessResponse(
                    result.Data!,
                    "User registered successfully. Please check your email to confirm your account."
                )
            );
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            var command = new ConfirmEmailCommand { Token = token };
            var success = await _mediator.Send(command);

            if (!success)
            {
                // Редирект на фронтенд с ошибкой
                return Redirect("http://localhost:3000/email-confirmed?success=false");
            }

            // Редирект на фронтенд с успехом
            return Redirect("http://localhost:3000/email-confirmed?success=true");
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return Unauthorized(ErrorResponse.Unauthorized(result.Error!));

            return Ok(ApiResponse<LoginResponse>.SuccessResponse(
                result.Data!,
                "Login successful"
            ));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken(
          [FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return Unauthorized(ErrorResponse.Unauthorized(result.Error!));

            return Ok(ApiResponse<LoginResponse>.SuccessResponse(
                result.Data!,
                "Token refreshed successfully"
            ));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
