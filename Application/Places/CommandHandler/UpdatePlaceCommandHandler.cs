using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Commands;
using Domain.Entities;
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

namespace Application.Places.CommandHandler
{
    public class UpdatePlaceCommandHandler : IRequestHandler<UpdatePlaceCommand, OperationResult<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UpdatePlaceCommandHandler> _logger;

        public UpdatePlaceCommandHandler(
            IApplicationDbContext context,
            IFileStorageService fileStorageService,
            ILogger<UpdatePlaceCommandHandler> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<OperationResult<PlaceDto>> Handle(
            UpdatePlaceCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Находим место
            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId && p.IsActive, cancellationToken);

            if (place == null)
                return OperationResult<PlaceDto>.Failure("Place not found");

            // 2. Обновляем только переданные поля
            if (request.Name != null) place.Name = request.Name;
            if (request.Description != null) place.Description = request.Description;
            if (request.CountryCode != null) place.CountryCode = request.CountryCode;
            if (request.City != null) place.City = request.City;
            if (request.Address != null) place.Address = request.Address;
            if (request.Latitude.HasValue) place.Latitude = request.Latitude;
            if (request.Longitude.HasValue) place.Longitude = request.Longitude;
            if (request.Category.HasValue) place.Category = request.Category.Value;
            if (request.PlaceType.HasValue) place.PlaceType = request.PlaceType.Value;

            // 3. AdditionalInfo — валидируем если передан
            if (request.AdditionalInfoJson != null)
            {
                if (string.IsNullOrWhiteSpace(request.AdditionalInfoJson))
                {
                    place.AdditionalInfo = null;
                }
                else
                {
                    try
                    {
                        JsonDocument.Parse(request.AdditionalInfoJson);
                        place.AdditionalInfo = request.AdditionalInfoJson;
                    }
                    catch (JsonException)
                    {
                        return OperationResult<PlaceDto>.Failure("Invalid JSON format in AdditionalInfo");
                    }
                }
            }

            place.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to update place {request.PlaceId}");
                return OperationResult<PlaceDto>.Failure(
                    $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }

            // 4. Обновляем Moods — только если переданы
            if (request.Moods != null)
            {
                // Удаляем старые
                var oldMoods = _context.PlaceMoods
                    .Where(m => m.PlaceId == place.PlaceId);
                _context.PlaceMoods.RemoveRange(oldMoods);

                // Добавляем новые
                if (request.Moods.Any())
                {
                    var newMoods = request.Moods
                        .Distinct()
                        .Select(m => new PlaceMood
                        {
                            PlaceId = place.PlaceId,
                            Mood = m
                        });
                    _context.PlaceMoods.AddRange(newMoods);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            // 5. Удаляем конкретные изображения
            if (request.DeleteImageIds != null && request.DeleteImageIds.Any())
            {
                var toDelete = await _context.Images
                    .Where(i => request.DeleteImageIds.Contains(i.ImageId)
                             && i.EntityId == place.PlaceId
                             && i.EntityType == ImageEntityType.Place)
                    .ToListAsync(cancellationToken);

                foreach (var img in toDelete)
                {
                    await _fileStorageService.DeleteFileAsync(img.ImageUrl);
                    img.IsActive = false; // soft delete
                }

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"🗑️ Deleted {toDelete.Count} images for place {place.PlaceId}");
            }

            // 6. Добавляем новые изображения
            if (request.NewImages != null && request.NewImages.Any())
            {
                try
                {
                    var uploaded = await _fileStorageService.UploadMultipleImagesAsync(
                        request.NewImages.ToArray(),
                        ImageEntityType.Place,
                        place.PlaceId,
                        request.UpdatedBy,
                        cancellationToken);

                    if (uploaded.Any())
                    {
                        _context.Images.AddRange(uploaded);
                        await _context.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation($"📸 Uploaded {uploaded.Count} new images for place {place.PlaceId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ Failed to upload images for place {place.PlaceId}");
                }
            }

            // 7. Смена обложки
            if (request.CoverImageId.HasValue)
            {
                // Снимаем флаг со всех
                var allImages = await _context.Images
                    .Where(i => i.EntityId == place.PlaceId
                             && i.EntityType == ImageEntityType.Place
                             && i.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var img in allImages)
                    img.IsCover = img.ImageId == request.CoverImageId.Value;

                await _context.SaveChangesAsync(cancellationToken);
            }

            // 8. Собираем актуальные изображения для ответа
            var images = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityId == place.PlaceId
                         && i.EntityType == ImageEntityType.Place
                         && i.IsActive)
                .OrderBy(i => i.SortOrder)
                .ToListAsync(cancellationToken);

            // 9. Собираем moods для ответа
            var moods = await _context.PlaceMoods
                .AsNoTracking()
                .Where(m => m.PlaceId == place.PlaceId)
                .Select(m => m.Mood)
                .ToListAsync(cancellationToken);

            // 10. Десериализуем AdditionalInfo
            PlaceAdditionalInfo? additionalInfo = null;
            if (!string.IsNullOrEmpty(place.AdditionalInfo))
            {
                try
                {
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    additionalInfo = place.Category switch
                    {
                        PlaceCategory.Food => JsonSerializer.Deserialize<FoodPlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Accommodation => JsonSerializer.Deserialize<AccommodationPlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Culture => JsonSerializer.Deserialize<CulturePlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Nature => JsonSerializer.Deserialize<NaturePlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Entertainment => JsonSerializer.Deserialize<EntertainmentPlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Shopping => JsonSerializer.Deserialize<ShoppingPlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Transport => JsonSerializer.Deserialize<TransportPlaceInfo>(place.AdditionalInfo, opts),
                        PlaceCategory.Services => JsonSerializer.Deserialize<ServicePlaceInfo>(place.AdditionalInfo, opts),
                        _ => null
                    };
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize AdditionalInfo");
                }
            }

            return OperationResult<PlaceDto>.Success(new PlaceDto
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
                Moods = moods,
                AdditionalInfo = additionalInfo,
                ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                CoverImageUrl = images.FirstOrDefault(i => i.IsCover)?.ImageUrl,
                CreatedAt = place.CreatedAt
            });
        }
    }
}
