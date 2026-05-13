using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
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
    public class UpdateTripMemberRoleCommandHandler
         : IRequestHandler<UpdateTripMemberRoleCommand, OperationResult<TripMemberDto>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTripMemberRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<TripMemberDto>> Handle(
            UpdateTripMemberRoleCommand request, CancellationToken cancellationToken)
        {
            var callerMember = await _context.TripMembers
                .FirstOrDefaultAsync(m =>
                    m.TripId == request.TripId && m.UserId == request.OwnerId,
                    cancellationToken);

            if (callerMember?.Role != TripMemberRole.Owner)
                return OperationResult<TripMemberDto>.Failure("Access denied. Only the trip owner can change roles.");

            if (request.NewRole == TripMemberRole.Owner)
                return OperationResult<TripMemberDto>.Failure("Cannot assign Owner role.");

            var target = await _context.TripMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m =>
                    m.TripId == request.TripId && m.UserId == request.TargetUserId,
                    cancellationToken);

            if (target is null)
                return OperationResult<TripMemberDto>.Failure("Member not found.");

            if (target.Role == TripMemberRole.Owner)
                return OperationResult<TripMemberDto>.Failure("Cannot change the owner's role.");

            target.Role = request.NewRole;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<TripMemberDto>.Success(new TripMemberDto
            {
                UserId = target.UserId,
                Username = target.User.Username,
                ProfilePicture = target.User.ProfilePicture,
                Name = target.User.Name,
                Role = target.Role,
                InvitedAt = target.InvitedAt,
            });
        }
    }
}
