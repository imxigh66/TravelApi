using Application.Common.Models;
using Application.DTO.Comment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands
{
    public class CreateCommentCommand:IRequest<OperationResult<CommentDto>>
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
    }
}
