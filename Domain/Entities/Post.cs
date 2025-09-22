using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int? PlaceId { get; set; }

        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // NAV
        public User User { get; set; } = null!;
        public Place? Place { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

}
