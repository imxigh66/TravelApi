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
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UnfollowUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            var fallow=await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == request.FollowerId 
                && uf.FollowingId == request.FollowingId, cancellationToken);

            if (fallow == null)
            {
                return OperationResult<bool>.Failure("You are not following this user.");
            }

            _context.UserFollows.Remove(fallow);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
