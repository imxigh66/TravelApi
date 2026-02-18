using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Queries;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Places.QueryHandler
{
    public class GetAllPlacesQueryHandler : IRequestHandler<GetAllPlacesQuery, PaginatedList<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPlacesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PlaceDto>> Handle(
            GetAllPlacesQuery request,
            CancellationToken cancellationToken)
        {
        
            var placesQuery = _context.Places
                .AsNoTracking()
                .Where(p => p.IsActive);

       

            var paginatedPlaces = await PaginatedList<Place>.CreateAsync(
                placesQuery.OrderByDescending(p => p.CreatedAt),
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

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

          
            var placeDtos = paginatedPlaces.Items.Select(p =>
            {
                // Десериализуем AdditionalInfo по категории
                object? additionalInfo = null;

                if (!string.IsNullOrEmpty(p.AdditionalInfo))
                {
                    try
                    {
                        additionalInfo = p.Category switch
                        {
                            PlaceCategory.Food => JsonSerializer.Deserialize<FoodPlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Accommodation => JsonSerializer.Deserialize<AccommodationPlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Culture => JsonSerializer.Deserialize<CulturePlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Nature => JsonSerializer.Deserialize<NaturePlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Entertainment => JsonSerializer.Deserialize<EntertainmentPlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Shopping => JsonSerializer.Deserialize<ShoppingPlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Transport => JsonSerializer.Deserialize<TransportPlaceInfo>(p.AdditionalInfo, jsonOptions),
                            PlaceCategory.Services => JsonSerializer.Deserialize<ServicePlaceInfo>(p.AdditionalInfo, jsonOptions),
                            _ => null
                        };
                    }
                    catch (JsonException)
                    {
                      
                    }
                }

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
                    AdditionalInfo = additionalInfo, 
                    ImageUrls = imagesByPlace.GetValueOrDefault(p.PlaceId) ?? new List<string>(),
                    CoverImageUrl = coverImages.GetValueOrDefault(p.PlaceId),
                    CreatedAt = p.CreatedAt
                };
            }).ToList();

            
            return new PaginatedList<PlaceDto>(
                placeDtos,
                paginatedPlaces.TotalCount,
                paginatedPlaces.PageNumber,
                paginatedPlaces.PageSize);
        }
    }
}