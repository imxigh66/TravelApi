using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.CategoryTags;
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
            var query = _context.Places
                .AsNoTracking()
                .Where(p => p.IsActive);

            // ── Фильтр по подборке модератора ──
            if (request.CategoryTagId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryTagLinks.Any(l => l.CategoryTagId == request.CategoryTagId.Value));
            }

            // ── Фильтр по категории ──
            if (request.Category.HasValue)
            {
                query = query.Where(p => p.Category == request.Category.Value);
            }

            // ── Фильтр по подтипу ──
            if (request.PlaceType.HasValue)
            {
                query = query.Where(p => p.PlaceType == request.PlaceType.Value);
            }

            // ── Фильтр по настроению ──
            if (request.Mood.HasValue)
            {
                query = query.Where(p =>
                    p.Moods.Any(m => m.Mood == request.Mood.Value));
            }

            // ── Фильтр по городу ──
            if (!string.IsNullOrEmpty(request.City))
            {
                query = query.Where(p =>
                    p.City.ToLower() == request.City.ToLower());
            }

            // ── Фильтр по стране ──
            if (!string.IsNullOrEmpty(request.CountryCode))
            {
                query = query.Where(p => p.CountryCode == request.CountryCode);
            }

            // ── Сортировка ──
            query = request.SortBy switch
            {
                "rating" => query.OrderByDescending(p => p.AverageRating),
                "popular" => query.OrderByDescending(p => p.ViewsCount),
                "saves" => query.OrderByDescending(p => p.SavesCount),
                _ => query.OrderByDescending(p => p.CreatedAt) // newest по умолчанию
            };

            // ── Пагинация ──
            var paginated = await PaginatedList<Place>.CreateAsync(
                query, request.PageNumber, request.PageSize, cancellationToken);

            if (!paginated.Items.Any())
                return new PaginatedList<PlaceDto>(
                    new List<PlaceDto>(), paginated.TotalCount,
                    paginated.PageNumber, paginated.PageSize);

            // ── Загружаем изображения ──
            var placeIds = paginated.Items.Select(p => p.PlaceId).ToList();

            var images = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Place
                         && placeIds.Contains(i.EntityId)
                         && i.IsActive)
                .OrderBy(i => i.EntityId).ThenBy(i => i.SortOrder)
                .ToListAsync(cancellationToken);

            var imagesByPlace = images
                .GroupBy(i => i.EntityId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ImageUrl).ToList());

            var coverImages = images
                .Where(i => i.IsCover)
                .GroupBy(i => i.EntityId)
                .ToDictionary(g => g.Key, g => g.First().ImageUrl);

            // ── Загружаем moods ──
            var moodsByPlace = await _context.PlaceMoods
                .AsNoTracking()
                .Where(m => placeIds.Contains(m.PlaceId))
                .GroupBy(m => m.PlaceId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(x => x.Mood).ToList(),
                    cancellationToken);

            // ── Загружаем теги подборок ──
            var tagsByPlace = await _context.CategoryTagLinks
                .AsNoTracking()
                .Where(l => placeIds.Contains(l.PlaceId))
                .Include(l => l.CategoryTag)
                .GroupBy(l => l.PlaceId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(x => new CategoryTagDto
                    {
                        CategoryTagId = x.CategoryTag.CategoryTagId,
                        Name = x.CategoryTag.Name,
                        Icon = x.CategoryTag.Icon
                    }).ToList(),
                    cancellationToken);

            // ── Маппинг в DTO ──
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var dtos = paginated.Items.Select(p =>
            {
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
                    catch (JsonException) { }
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
                    Moods = moodsByPlace.GetValueOrDefault(p.PlaceId) ?? new List<MoodType>(),
                    CategoryTags = tagsByPlace.GetValueOrDefault(p.PlaceId) ?? new List<CategoryTagDto>(),
                    ImageUrls = imagesByPlace.GetValueOrDefault(p.PlaceId) ?? new List<string>(),
                    CoverImageUrl = coverImages.GetValueOrDefault(p.PlaceId),
                    CreatedAt = p.CreatedAt
                };
            }).ToList();

            return new PaginatedList<PlaceDto>(
                dtos, paginated.TotalCount,
                paginated.PageNumber, paginated.PageSize);
        }
    }
}