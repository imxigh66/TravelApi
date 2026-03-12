using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.Trips.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.QueryHandler
{
    public class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, OperationResult<TripDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTripByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<TripDetailDto>> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
            .Include(t => t.Owner)
            .Include(t => t.TripPlaces)
                .ThenInclude(tp => tp.Place)
            .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
            {
                return OperationResult<TripDetailDto>.Failure("Trip not found.");
            }

            if (!trip.IsPublic)
            {
                return OperationResult<TripDetailDto>.Failure("Trip is not public.");
            }

            var placeIds = trip.TripPlaces.Select(tp => tp.PlaceId).ToList();
            var coverImages = await _context.Images
                .Where(i => placeIds.Contains(i.EntityId)
                         && i.EntityType == ImageEntityType.Place
                         && i.IsCover)
                .ToDictionaryAsync(i => i.EntityId, i => i.ImageUrl, cancellationToken);
            var dto = new TripDetailDto
            {
                TripId = trip.TripId,
                Title = trip.Title,
                Description = trip.Description,
                City = trip.City,
                CountryCode = trip.CountryCode,
                TripDate = trip.TripDate,
                IsPublic = trip.IsPublic,
                Status = trip.Status,
                CreatedAt = trip.CreatedAt,
                PlacesCount = trip.TripPlaces.Count,
                OwnerId = trip.OwnerId,
                OwnerUsername = trip.Owner.Username,
                OwnerProfilePicture = trip.Owner.ProfilePicture,
                CoverImageUrl = trip.TripPlaces
                .OrderBy(tp => tp.SortOrder)
                .Select(tp => coverImages.GetValueOrDefault(tp.PlaceId))
                .FirstOrDefault(url => url != null),
                Places = trip.TripPlaces
                .OrderBy(tp => tp.SortOrder)
                .Select(tp => new TripPlaceDto
                {
                    PlaceId = tp.PlaceId,
                    Name = tp.Place.Name,
                    City = tp.Place.City,
                    Address = tp.Place.Address,
                    Notes = tp.Notes,
                    SortOrder = tp.SortOrder,
                    CoverImageUrl = coverImages.GetValueOrDefault(tp.PlaceId)
                })
                .ToList()
            };

            return OperationResult<TripDetailDto>.Success(dto);
        }
    }
}
