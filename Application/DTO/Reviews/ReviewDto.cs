using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Reviews
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? UserProfilePicture { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
