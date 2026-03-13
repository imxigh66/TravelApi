using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Trips.Commands;
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

            if (trip.OwnerId != request.UserId)
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
