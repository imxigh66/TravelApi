using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TripMember
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public TripMemberRole Role { get; set; }
        public DateTime InvitedAt { get; set; }

        // Navigation
        public Trip Trip { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
