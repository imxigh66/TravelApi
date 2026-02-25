using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Follows.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Follows.CommandHandler
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public FollowUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if(request.FollowerId == request.FollowingId)
            {
                return OperationResult<bool>.Failure("You cannot follow yourself.");
            }

            var targetExists= await _context.Users.AnyAsync(u=>u.UserId == request.FollowingId, cancellationToken);

            if(!targetExists) { return OperationResult<bool>.Failure("The user you are trying to follow does not exist."); }
                

            var alreadyFollowing = await _context.UserFollows
                .AnyAsync(uf => uf.FollowerId == request.FollowerId && 
                uf.FollowingId == request.FollowingId, cancellationToken);

            if(alreadyFollowing)
            {
                return OperationResult<bool>.Failure("You are already following this user.");
            }

            var follow = new Domain.Entities.UserFollow
            {
                FollowerId = request.FollowerId,
                FollowingId = request.FollowingId,
                FollowedAt = DateTime.UtcNow
            };

            _context.UserFollows.Add(follow);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
