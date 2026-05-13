using Application.Common.Models;
using Application.DTO.Messages;
using Application.Messages.Commands;
using Application.Messages.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/conversations
        [HttpGet]
        public async Task<ActionResult<List<ConversationDto>>> GetConversations()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetConversationsQuery { UserId = userId });
            return Ok(result);
        }

        // POST /api/conversations  — начать диалог с пользователем
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ConversationDto>>> StartConversation(
            [FromBody] StartConversationRequest dto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _mediator.Send(new StartConversationCommand
            {
                CurrentUserId = userId,
                OtherUserId = dto.OtherUserId,
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<ConversationDto>.SuccessResponse(result.Data!));
        }

        // GET /api/conversations/{id}/messages
        [HttpGet("{id:int}/messages")]
        public async Task<ActionResult<PaginatedList<MessageDto>>> GetMessages(
            int id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 30)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetMessagesQuery
            {
                ConversationId = id,
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });

            return Ok(result);
        }

        // POST /api/conversations/{id}/messages — REST fallback (обычно через SignalR)
        [HttpPost("{id:int}/messages")]
        public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage(
            int id,
            [FromBody] SendMessageRequest dto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _mediator.Send(new SendMessageCommand
            {
                ConversationId = id,
                SenderId = userId,
                Content = dto.Content,
            });

            if (!result.IsSuccess)
                return BadRequest(ErrorResponse.BadRequest(result.Error!));

            return Ok(ApiResponse<MessageDto>.SuccessResponse(result.Data!));
        }

        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out userId);
        }

        public record StartConversationRequest(int OtherUserId);
        public record SendMessageRequest(string Content);
    }
}
