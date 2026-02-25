using Application.Common.Models;
using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Follows.Queries
{
    public class GetFollowersQuery : IRequest<PaginatedList<UserFollowDto>>
    {
        public int UserId { get; set; } // чьи подписчики
        public int CurrentUserId { get; set; } // для isFollowing
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
