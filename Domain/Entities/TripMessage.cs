using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TripMessage
    {
        public int TripMessageId { get; set; }
        public int TripId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Trip Trip { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
