using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Follows.Commands
{
    public class FollowUserCommand : IRequest<OperationResult<bool>>
    {
        public int FollowerId { get; set; } // из JWT
        public int FollowingId { get; set; } // из роута
    }
}
