using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Queries;
using Domain.Entities;
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
    public class GetPendingPlacesQueryHandler
        : IRequestHandler<GetPendingPlacesQuery, PaginatedList<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingPlacesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PlaceDto>> Handle(
            GetPendingPlacesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Places
                .AsNoTracking()
                .Where(p => p.Status == PlaceStatus.Pending)
                .OrderByDescending(p => p.CreatedAt);

            var paginated = await PaginatedList<Place>.CreateAsync(
                query, request.PageNumber, request.PageSize, cancellationToken);

            if (!paginated.Items.Any())
                return new PaginatedList<PlaceDto>(
                    new List<PlaceDto>(), paginated.TotalCount,
                    paginated.PageNumber, paginated.PageSize);

         
            var placeIds = paginated.Items.Select(p => p.PlaceId).ToList();

            var images = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Place
                         && placeIds.Contains(i.EntityId)
                         && i.IsActive)
                .OrderBy(i => i.EntityId)
                .ThenBy(i => i.SortOrder)
                .ToListAsync(cancellationToken);

            var imagesByPlace = images
                .GroupBy(i => i.EntityId)
                .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

            var coverImages = images
                .GroupBy(i => i.EntityId)
                .ToDictionary(g => g.Key, g => g.First().ImageUrl);

     
            var creatorIds = paginated.Items
                .Where(p => p.CreatedBy.HasValue)
                .Select(p => p.CreatedBy!.Value)
                .Distinct()
                .ToList();

            var creators = await _context.Users
                .AsNoTracking()
                .Where(u => creatorIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, cancellationToken);

            var dtos = paginated.Items.Select(p =>
            {
                creators.TryGetValue(p.CreatedBy ?? 0, out var creator);
                return new PlaceDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    CountryCode = p.CountryCode,
                    City = p.City,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Category = p.Category,
                    PlaceType = p.PlaceType,
                    AverageRating = p.AverageRating,
                    ReviewsCount = p.ReviewsCount,
                    SavesCount = p.SavesCount,
                    ImageUrls = imagesByPlace.GetValueOrDefault(p.PlaceId) ?? new List<string>(),
                    CoverImageUrl = coverImages.GetValueOrDefault(p.PlaceId),
                    CreatedBy = p.CreatedBy,
                    CreatorUsername = creator?.Username,
                    CreatorProfilePicture = creator?.ProfilePicture,
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    RejectionReason = p.RejectionReason,
                };
            }).ToList();

            return new PaginatedList<PlaceDto>(
                dtos, paginated.TotalCount,
                paginated.PageNumber, paginated.PageSize);
        }
    }
}
