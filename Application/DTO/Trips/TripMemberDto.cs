using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripMemberDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public string? Name { get; set; }
        public TripMemberRole Role { get; set; }
        public DateTime InvitedAt { get; set; }
    }
}
