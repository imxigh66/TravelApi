using Application.DTO.Notifications;
using Application.Notifications.Commands;
using Application.Notifications.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/notifications?pageSize=20
        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
            [FromQuery] int pageSize = 20)
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var result = await _mediator.Send(new GetNotificationsQuery
            {
                UserId = userId,
                PageSize = pageSize,
            });

            return Ok(result);
        }

        // GET /api/notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            var count = await _mediator.Send(new GetUnreadCountQuery { UserId = userId });
            return Ok(new { count });
        }

        // PATCH /api/notifications/read-all
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            if (!TryGetUserId(out int userId)) return Unauthorized();

            await _mediator.Send(new MarkNotificationsReadCommand { UserId = userId });
            return NoContent();
        }

        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(claim, out userId);
        }
    }
}
