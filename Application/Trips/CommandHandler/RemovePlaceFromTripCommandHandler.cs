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
    public class RemovePlaceFromTripCommandHandler: IRequestHandler<RemovePlaceFromTripCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public RemovePlaceFromTripCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(RemovePlaceFromTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<bool>.Failure("Trip not found.");

            var member = await _context.TripMembers
    .FirstOrDefaultAsync(m => m.TripId == request.TripId && m.UserId == request.UserId, cancellationToken);

            if (member is null || member.Role == TripMemberRole.Viewer)
                return OperationResult<bool>.Failure("Access denied.");

            var tripPlace = await _context.TripPlaces
                .FirstOrDefaultAsync(tp => tp.TripId == request.TripId
                                        && tp.PlaceId == request.PlaceId, cancellationToken);

            if (tripPlace == null)
                return OperationResult<bool>.Failure("Place not found in trip.");

            _context.TripPlaces.Remove(tripPlace);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
