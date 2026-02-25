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
    public class GetFollowingQuery : IRequest<PaginatedList<UserFollowDto>>
    {
        public int UserId { get; set; } // чьи подписки
        public int CurrentUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
