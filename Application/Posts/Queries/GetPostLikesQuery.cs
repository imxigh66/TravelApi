using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Queries
{
    public class GetPostLikesQuery : IRequest<List<UserDto>>
    {
        public int PostId { get; set; }
    }
}
