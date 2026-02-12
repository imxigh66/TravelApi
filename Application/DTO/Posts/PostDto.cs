using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Posts
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }

        public string Username { get; set; } = null!;
        public string? UserProfilePicture { get; set; }
        public int? PlaceId { get; set; }

        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public List<string> ImageUrls { get; set; } = new List<string>();

        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
