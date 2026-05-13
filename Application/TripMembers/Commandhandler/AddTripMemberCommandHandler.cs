using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.TripMembers.Commands;
using Domain.Entities;
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
    public class AddTripMemberCommandHandler
         : IRequestHandler<AddTripMemberCommand, OperationResult<TripMemberDto>>
    {
        private readonly IApplicationDbContext _context;

        public AddTripMemberCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<TripMemberDto>> Handle(
            AddTripMemberCommand request, CancellationToken cancellationToken)
        {
            // Только Owner может приглашать
            var callerMember = await _context.TripMembers
                .FirstOrDefaultAsync(m =>
                    m.TripId == request.TripId && m.UserId == request.OwnerId,
                    cancellationToken);

            if (callerMember is null || callerMember.Role != TripMemberRole.Owner)
                return OperationResult<TripMemberDto>.Failure("Access denied. Only the trip owner can add members.");

            // Нельзя добавить себя
            if (request.OwnerId == request.TargetUserId)
                return OperationResult<TripMemberDto>.Failure("You are already a member of this trip.");

            // Нельзя назначить роль Owner через этот эндпоинт
            if (request.Role == TripMemberRole.Owner)
                return OperationResult<TripMemberDto>.Failure("Cannot assign Owner role.");

            // Проверяем что такой юзер существует
            var targetUser = await _context.Users.FindAsync(
                new object[] { request.TargetUserId }, cancellationToken);

            if (targetUser is null)
                return OperationResult<TripMemberDto>.Failure("User not found.");

            // Уже участник?
            var alreadyMember = await _context.TripMembers
                .AnyAsync(m => m.TripId == request.TripId && m.UserId == request.TargetUserId,
                    cancellationToken);

            if (alreadyMember)
                return OperationResult<TripMemberDto>.Failure("User is already a member of this trip.");

            var member = new TripMember
            {
                TripId = request.TripId,
                UserId = request.TargetUserId,
                Role = request.Role,
                InvitedAt = DateTime.UtcNow,
            };

            _context.TripMembers.Add(member);
            await _context.SaveChangesAsync(cancellationToken);

            _context.Notifications.Add(new Domain.Entities.Notification
            {
                RecipientId = request.TargetUserId,
                ActorId = request.OwnerId,
                Type = Domain.Enum.NotificationType.TripInvite,
                Message = "добавил(а) вас в поездку",
                Link = $"/trips/{request.TripId}",
                CreatedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<TripMemberDto>.Success(new TripMemberDto
            {
                UserId = targetUser.UserId,
                Username = targetUser.Username,
                ProfilePicture = targetUser.ProfilePicture,
                Name = targetUser.Name,
                Role = request.Role,
                InvitedAt = member.InvitedAt,
            });
        }
    }
}
