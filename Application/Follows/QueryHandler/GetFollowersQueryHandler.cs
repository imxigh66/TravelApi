using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Follows.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Follows.QueryHandler
{
    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PaginatedList<UserFollowDto>>
    {

        private readonly IApplicationDbContext _context;

        public GetFollowersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<UserFollowDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var currentUserFollowingIds = _context.UserFollows
                .Where(uf => uf.FollowerId == request.CurrentUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync(cancellationToken);

            var query=_context.UserFollows
                .AsNoTracking()
                .Where(uf => uf.FollowingId == request.UserId)
                .OrderByDescending(uf => uf.FollowedAt)
                .Select(f=> new UserFollowDto
                {
                    UserId = f.Follower.UserId,
                    Username = f.Follower.Username,
                    Name = f.Follower.Name,
                    ProfilePicture = f.Follower.ProfilePicture,
                    Bio = f.Follower.Bio,
                    IsFollowing = currentUserFollowingIds.Result.Contains(f.Follower.UserId)
                });

            return await PaginatedList<UserFollowDto>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
