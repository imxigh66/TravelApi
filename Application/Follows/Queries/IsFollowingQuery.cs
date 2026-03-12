using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Follows.Queries
{
    public class IsFollowingQuery : IRequest<bool>
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
    }
}
