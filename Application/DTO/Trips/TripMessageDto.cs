using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripMessageDto
    {
        public int TripMessageId { get; set; }
        public int TripId { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = null!;
        public string? SenderProfilePicture { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
