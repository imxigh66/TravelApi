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
    public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, PaginatedList<UserFollowDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFollowingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<UserFollowDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
        {
            var currentUserFollowingIds = await _context.UserFollows
                .Where(f => f.FollowerId == request.CurrentUserId)
                .Select(f => f.FollowingId)
                .ToListAsync(cancellationToken);

            var query = _context.UserFollows
                .AsNoTracking()
                .Where(f => f.FollowerId == request.UserId) 
                .OrderByDescending(f => f.FollowedAt)
                .Select(f => new UserFollowDto
                {
                    UserId = f.Following.UserId,
                    Username = f.Following.Username,
                    Name = f.Following.Name,
                    ProfilePicture = f.Following.ProfilePicture,
                    Bio = f.Following.Bio,
                    IsFollowing = currentUserFollowingIds.Contains(f.Following.UserId)
                });

            return await PaginatedList<UserFollowDto>.CreateAsync(
                query, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
