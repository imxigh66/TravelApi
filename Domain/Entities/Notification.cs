using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        /// <summary>Кому адресовано уведомление</summary>
        public int RecipientId { get; set; }

        /// <summary>Кто инициировал (null = системное)</summary>
        public int? ActorId { get; set; }

        public NotificationType Type { get; set; }

        /// <summary>Текст уведомления, например "lara лайкнула ваш пост"</summary>
        public string Message { get; set; } = null!;

        /// <summary>Ссылка для перехода (например /posts/12)</summary>
        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User Recipient { get; set; } = null!;
        public User? Actor { get; set; }
    }
}
