using Application.Common.Interfaces;
using Application.Common.Models;
using Application.TripMembers.Commands;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripMembers.Commandhandler
{
    public class RemoveTripMemberCommandHandler
        : IRequestHandler<RemoveTripMemberCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;

        public RemoveTripMemberCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<bool>> Handle(
            RemoveTripMemberCommand request, CancellationToken cancellationToken)
        {
            var callerMember = await _context.TripMembers
                .FirstOrDefaultAsync(m =>
                    m.TripId == request.TripId && m.UserId == request.RequesterId,
                    cancellationToken);

            // Owner может удалять кого угодно; участник может удалить только себя
            var isSelf = request.RequesterId == request.TargetUserId;
            var isOwner = callerMember?.Role == TripMemberRole.Owner;

            if (!isSelf && !isOwner)
                return OperationResult<bool>.Failure("Access denied.");

            // Нельзя удалить Owner
            var targetMember = await _context.TripMembers
                .FirstOrDefaultAsync(m =>
                    m.TripId == request.TripId && m.UserId == request.TargetUserId,
                    cancellationToken);

            if (targetMember is null)
                return OperationResult<bool>.Failure("Member not found.");

            if (targetMember.Role == TripMemberRole.Owner)
                return OperationResult<bool>.Failure("Cannot remove the trip owner.");

            _context.TripMembers.Remove(targetMember);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
