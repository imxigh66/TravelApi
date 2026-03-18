using Application.Common.Interfaces;
using Application.DTO.Trips;
using Application.DTO.Trips.Destination;
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
    public class GetMyTripsQueryHandler : IRequestHandler<GetMyTripsQuery, List<TripDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetMyTripsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<TripDto>> Handle(GetMyTripsQuery request, CancellationToken cancellationToken)
        {
            var trips = await _context.Trips
                .Where(t => t.OwnerId == request.OwnerId)
                .OrderByDescending(t => t.CreatedAt)
                .Include(t => t.TripPlaces)
                .Include(t => t.Destinations)   
                .ToListAsync(cancellationToken);

            var placeIds = trips
                .SelectMany(t => t.TripPlaces)
                .Select(tp => tp.PlaceId)
                .Distinct()
                .ToList();

            var coverImages = placeIds.Any()
                ? await _context.Images
                    .Where(i => placeIds.Contains(i.EntityId)
                             && i.EntityType == ImageEntityType.Place
                             && i.IsCover)
                    .ToDictionaryAsync(i => i.EntityId, i => i.ImageUrl, cancellationToken)
                : new Dictionary<int, string>();

            return trips.Select(t => new TripDto
            {
                TripId = t.TripId,
                Title = t.Title,
                Description = t.Description,
                City = t.City,
                CountryCode = t.CountryCode,
                TripDate = t.TripDate,
                IsPublic = t.IsPublic,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                PlacesCount = t.TripPlaces.Count,
                CoverImageUrl = t.TripPlaces
                    .OrderBy(tp => tp.SortOrder)
                    .Select(tp => coverImages.GetValueOrDefault(tp.PlaceId))
                    .FirstOrDefault(url => url != null),
                Destinations = t.Destinations   
                    .OrderBy(d => d.SortOrder)
                    .Select(d => new TripDestinationDto
                    {
                        Id = d.Id,
                        TripId = d.TripId,
                        City = d.City,
                        CountryCode = d.CountryCode,
                        SortOrder = d.SortOrder,
                        DateFrom = d.DateFrom,
                        DateTo = d.DateTo,
                    })
                    .ToList()
            }).ToList();
        }
    }
}
