using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Messages
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }

        // Собеседник (не текущий пользователь)
        public int OtherUserId { get; set; }
        public string OtherUsername { get; set; } = null!;
        public string? OtherUserProfilePicture { get; set; }

        public string? LastMessageText { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
