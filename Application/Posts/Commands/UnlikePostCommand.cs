using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Commands
{
    public class UnlikePostCommand:IRequest<bool>
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
}
