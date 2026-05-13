using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Notifications
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int? ActorId { get; set; }
        public string? ActorUsername { get; set; }
        public string? ActorProfilePicture { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = null!;
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
