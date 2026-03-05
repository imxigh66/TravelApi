using Application.Common.Models;
using Application.DTO.Posts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Queries
{
    public class GetFeedQuery : IRequest<PaginatedList<PostDto>>
    {
        public int CurrentUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
