using Application.Common.Models;
using Application.DTO.Comment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Queries
{
    public class GetCommentsQuery:IRequest<PaginatedList<CommentDto>>
    {
        public int PostId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
