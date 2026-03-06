using Application.DTO.Comment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands
{
    public class DeleteCommentCommand:IRequest<bool>
    {
        public int CommentId { get; set; }
        public int UserId { get; set; } 
    }
}
