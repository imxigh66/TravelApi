using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Posts
{
    public class CreatePostWithImageDto
    {
        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public int? PlaceId { get; set; }
        public List<IFormFile>? Images { get; set; }  
    }
}
