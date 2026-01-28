using Application.Common.Models;
using Application.DTO.Posts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Commands
{
    public class CreatePostCommand:IRequest<OperationResult<PostDto>>
    {
        public int UserId { get; set; }
        public int? PlaceId { get; set; }

        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public int LikesCount { get; set; }
    }
}
