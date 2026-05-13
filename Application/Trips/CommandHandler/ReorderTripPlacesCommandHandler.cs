using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.Trips.Commands;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class ReorderTripPlacesCommandHandler: IRequestHandler<ReorderTripPlacesCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public ReorderTripPlacesCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(ReorderTripPlacesCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<bool>.Failure("Trip not found.");

            var member = await _context.TripMembers
    .FirstOrDefaultAsync(m => m.TripId == request.TripId && m.UserId == request.UserId, cancellationToken);

            if (member is null || member.Role == TripMemberRole.Viewer)
                return OperationResult<bool>.Failure("Access denied.");

            var tripPlaces = await _context.TripPlaces
                .Where(tp => tp.TripId == request.TripId)
                .ToListAsync(cancellationToken);

            foreach (var item in request.Places)
            {
                var tripPlace = tripPlaces.FirstOrDefault(tp => tp.PlaceId == item.PlaceId);
                if (tripPlace != null)
                    tripPlace.SortOrder = item.SortOrder;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
