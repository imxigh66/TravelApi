using Application.DTO.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.Queries
{
    public class GetNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public int UserId { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
