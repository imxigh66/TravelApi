using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }

        public int User1Id { get; set; }
        public int User2Id { get; set; }

        /// <summary>Текст последнего сообщения — для превью в списке</summary>
        public string? LastMessageText { get; set; }

        public DateTime? LastMessageAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
