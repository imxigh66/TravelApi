using Application.Common.Interfaces;
using Application.DTO.Trips;
using Application.TripMembers.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripMembers.Queryhandler
{
    public class GetTripMembersQueryHandler
         : IRequestHandler<GetTripMembersQuery, List<TripMemberDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTripMembersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TripMemberDto>> Handle(
            GetTripMembersQuery request, CancellationToken cancellationToken)
        {
            // Проверяем что requester — участник или трип публичный
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip is null) return [];

            var isMember = await _context.TripMembers
                .AnyAsync(m => m.TripId == request.TripId && m.UserId == request.RequesterId,
                    cancellationToken);

            if (!trip.IsPublic && !isMember) return [];

            return await _context.TripMembers
                .Include(m => m.User)
                .Where(m => m.TripId == request.TripId)
                .OrderBy(m => m.Role)
                .ThenBy(m => m.InvitedAt)
                .Select(m => new TripMemberDto
                {
                    UserId = m.UserId,
                    Username = m.User.Username,
                    ProfilePicture = m.User.ProfilePicture,
                    Name = m.User.Name,
                    Role = m.Role,
                    InvitedAt = m.InvitedAt,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
