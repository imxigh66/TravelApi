using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Places.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class GetAllPlacesQueryHandler : IRequestHandler<GetAllPlacesQuery, PaginatedList<PlaceDto>> {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllPlacesQueryHandler(IApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<PlaceDto>> Handle(GetAllPlacesQuery request, CancellationToken cancellationToken)
        {

            var placesQuery = _context.Places
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt);

            var paginatedPlaces = await PaginatedList<Place>.CreateAsync(
                placesQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            if (!paginatedPlaces.Items.Any())
            {
                return new PaginatedList<PlaceDto>(
                    new List<PlaceDto>(),
                    paginatedPlaces.TotalCount,
                    paginatedPlaces.PageNumber,
                    paginatedPlaces.PageSize);
            }

          
            var placeIds = paginatedPlaces.Items.Select(p => p.PlaceId).ToList();

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
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ImageUrl).ToList());

            var coverImages = images
                .Where(i => i.IsCover)
                .GroupBy(i => i.EntityId)
                .ToDictionary(g => g.Key, g => g.First().ImageUrl);

       
            var placeDtos = paginatedPlaces.Items.Select(p => new PlaceDto
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
                CreatedAt = p.CreatedAt
            }).ToList();

            // 8. Возвращаем пагинированный результат
            return new PaginatedList<PlaceDto>(
                placeDtos,
                paginatedPlaces.TotalCount,
                paginatedPlaces.PageNumber,
                paginatedPlaces.PageSize);
        }
    }
}
