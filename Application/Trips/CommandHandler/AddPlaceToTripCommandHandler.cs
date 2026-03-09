using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.Trips.Commands;
using Domain.Entities;
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
    public class AddPlaceToTripCommandHandler : IRequestHandler<AddPlaceToTripCommand, OperationResult<TripPlaceDto>>
    {
        private readonly IApplicationDbContext _context;
        public AddPlaceToTripCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<TripPlaceDto>> Handle(AddPlaceToTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
    .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<TripPlaceDto>.Failure("Trip not found.");

            if (trip.OwnerId != request.UserId)
                return OperationResult<TripPlaceDto>.Failure("Access denied.");

            var tripPlaces = await _context.TripPlaces
                .Where(tp => tp.TripId == request.TripId)
                .ToListAsync(cancellationToken);

           
            if (tripPlaces.Any(tp => tp.PlaceId == request.PlaceId))
                return OperationResult<TripPlaceDto>.Failure("Place already in trip.");

            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId, cancellationToken);

            if (place == null)
                return OperationResult<TripPlaceDto>.Failure("Place not found.");

            var sortOrder = trip.TripPlaces.Any()
                ? trip.TripPlaces.Max(tp => tp.SortOrder) + 1
                : 1;

            var tripPlace = new TripPlace
            {
                TripId = request.TripId,
                PlaceId = request.PlaceId,
                Notes = request.Notes,
                SortOrder = sortOrder
            };

            _context.TripPlaces.Add(tripPlace);
            await _context.SaveChangesAsync(cancellationToken);

            var coverImageUrl = await _context.Images
                .Where(i => i.EntityId == request.PlaceId
                         && i.EntityType == ImageEntityType.Place
                         && i.IsCover)
                .Select(i => i.ImageUrl)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = new TripPlaceDto
            {
                PlaceId = place.PlaceId,
                Name = place.Name,
                City = place.City,
                Address = place.Address,
                Notes = request.Notes,
                SortOrder = sortOrder,
                CoverImageUrl = coverImageUrl
            };

            return OperationResult<TripPlaceDto>.Success(dto);
        }
    }
}
