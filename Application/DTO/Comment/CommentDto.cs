using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? UserProfilePicture { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
