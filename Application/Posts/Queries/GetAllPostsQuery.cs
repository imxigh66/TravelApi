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
    public class GetAllPostsQuery:IRequest<PaginatedList<PostDto>>
    {
        public int UserId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
