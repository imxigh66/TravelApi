using Application.Common.Interfaces;
using Application.DTO.Notifications;
using Application.Notifications.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.QueryHandler
{
    public class GetNotificationsQueryHandler
       : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetNotificationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDto>> Handle(
            GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == request.UserId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(request.PageSize)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    ActorId = n.ActorId,
                    ActorUsername = n.Actor != null ? n.Actor.Username : null,
                    ActorProfilePicture = n.Actor != null ? n.Actor.ProfilePicture : null,
                    Type = n.Type,
                    Message = n.Message,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
