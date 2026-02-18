using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class GetPlaceByIdQueryHandler : IRequestHandler<GetPlaceByIdQuery, OperationResult<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetPlaceByIdQueryHandler> _logger;
        public GetPlaceByIdQueryHandler(IApplicationDbContext context, ILogger<GetPlaceByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<OperationResult<PlaceDto>> Handle(GetPlaceByIdQuery request, CancellationToken cancellationToken)
        {

            var place = await _context.Places
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId && p.IsActive, cancellationToken);

            if (place == null)
            {
                return OperationResult<PlaceDto>.Failure("Place not found");
            }

            // Загружаем изображения
            var images = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Place
                         && i.EntityId == place.PlaceId
                         && i.IsActive)
                .OrderBy(i => i.SortOrder)
                .ToListAsync(cancellationToken);

            PlaceAdditionalInfo? additionalInfo = null;

            if (!string.IsNullOrEmpty(place.AdditionalInfo))
            {
                _logger.LogInformation($"🔍 Deserializing AdditionalInfo for place {place.PlaceId}");
                _logger.LogInformation($"📝 JSON: {place.AdditionalInfo}");
                _logger.LogInformation($"📂 Category: {place.Category}");

                try
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // ✅ ОБЯЗАТЕЛЬНО
                    };

                    additionalInfo = place.Category switch
                    {
                        PlaceCategory.Food => JsonSerializer.Deserialize<FoodPlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Accommodation => JsonSerializer.Deserialize<AccommodationPlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Culture => JsonSerializer.Deserialize<CulturePlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Nature => JsonSerializer.Deserialize<NaturePlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Entertainment => JsonSerializer.Deserialize<EntertainmentPlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Shopping => JsonSerializer.Deserialize<ShoppingPlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Transport => JsonSerializer.Deserialize<TransportPlaceInfo>(place.AdditionalInfo, jsonOptions),
                        PlaceCategory.Services => JsonSerializer.Deserialize<ServicePlaceInfo>(place.AdditionalInfo, jsonOptions),
                        _ => null
                    };

                    if (additionalInfo != null)
                    {
                        _logger.LogInformation($"✅ Successfully deserialized AdditionalInfo");

                        // Проверим содержимое для Food
                        if (additionalInfo is FoodPlaceInfo foodInfo)
                        {
                            _logger.LogInformation($"   Cuisine: {foodInfo.Cuisine}");
                            _logger.LogInformation($"   PriceRange: {foodInfo.PriceRange}");
                            _logger.LogInformation($"   HasDelivery: {foodInfo.HasDelivery}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Deserialization returned null");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"❌ Failed to deserialize AdditionalInfo");
                    _logger.LogError($"   JSON was: {place.AdditionalInfo}");
                }
            }
            else
            {
                _logger.LogInformation($"ℹ️ No AdditionalInfo for place {place.PlaceId}");
            }

            var dto = new PlaceDto
            {
                PlaceId = place.PlaceId,
                Name = place.Name,
                Description = place.Description,
                CountryCode = place.CountryCode,
                City = place.City,
                Address = place.Address,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                Category = place.Category,
                PlaceType = place.PlaceType,
                AverageRating = place.AverageRating,
                ReviewsCount = place.ReviewsCount,
                SavesCount = place.SavesCount,
                AdditionalInfo = additionalInfo, // ← Типизированный объект
                ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                CoverImageUrl = images.FirstOrDefault(i => i.IsCover)?.ImageUrl,
                CreatedAt = place.CreatedAt
            };

            return OperationResult<PlaceDto>.Success(dto);

        }
    }
}
