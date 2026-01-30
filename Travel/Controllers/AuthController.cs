using Application.Auth.Login.Commands;
using Application.Auth.RefreshToken.Commands;
using Application.Auth.Register.Commands;
using Application.Common.Models;
using Application.DTO.Auth;
using MediatR;
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
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return CreatedAtAction(
                nameof(Register),
                ApiResponse<RegisterDto>.SuccessResponse(
                    result.Data!,
                    "User registered successfully"
                )
            );
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
    }
}
