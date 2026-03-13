using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class GetNearbyQueryHandler : IRequestHandler<GetNearbyQuery, OperationResult<List<PlaceDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetNearbyQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<List<PlaceDto>>> Handle(GetNearbyQuery request, CancellationToken cancellationToken)
        {
            var place = await _context.Places
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId);

            if (place == null) return  OperationResult<List<PlaceDto>>.Failure("Place not found");

            var nearby = await _context.Places
            .AsNoTracking()
            .Where(p => p.PlaceId != request.PlaceId
                     && p.IsActive
                     && p.City == place.City
                     && p.Category == place.Category)
            .OrderByDescending(p => p.AverageRating)
            .Take(6)
            .ToListAsync(cancellationToken);

            var placeIds = nearby.Select(p => p.PlaceId).ToList();

            var coverImages = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Place
                         && placeIds.Contains(i.EntityId)
                         && i.IsCover
                         && i.IsActive)
                .GroupBy(i => i.EntityId)
                .ToDictionaryAsync(g => g.Key, g => g.First().ImageUrl, cancellationToken);

            var dtos = nearby.Select(p => new PlaceDto
            {
                PlaceId = p.PlaceId,
                Name = p.Name,
                Description = p.Description,
                City = p.City,
                Category = p.Category,
                PlaceType = p.PlaceType,
                AverageRating = p.AverageRating,
                ReviewsCount = p.ReviewsCount,
                CoverImageUrl = coverImages.GetValueOrDefault(p.PlaceId),
                ImageUrls = new List<string>(),
                Moods = new List<MoodType>()
            }).ToList();

            return OperationResult<List<PlaceDto>>.Success(dtos);
        }
    }
}
