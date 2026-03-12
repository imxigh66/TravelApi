using Application.Common.Interfaces;
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
    public class IsFollowingQueryHandler : IRequestHandler<IsFollowingQuery, bool>
    {
        private readonly IApplicationDbContext _context;

        public IsFollowingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
        {
            return await _context.UserFollows
                .AnyAsync(f => f.FollowerId == request.FollowerId
                            && f.FollowingId == request.FollowingId, cancellationToken);
        }
    }
}
