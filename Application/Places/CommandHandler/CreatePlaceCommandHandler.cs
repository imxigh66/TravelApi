using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Auth;
using Application.DTO.Places;
using Application.Places.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using MediatR.Pipeline;
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
    public class CreatePlaceCommandHandler : IRequestHandler<CreatePlaceCommand, OperationResult<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<CreatePlaceCommandHandler> _logger;

        public CreatePlaceCommandHandler(
            IApplicationDbContext context,
            IFileStorageService fileStorageService,
            ILogger<CreatePlaceCommandHandler> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<OperationResult<PlaceDto>> Handle(
            CreatePlaceCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Проверяем дубликаты
            var exists = await _context.Places
                .AnyAsync(p => p.Name == request.Name
                            && p.City == request.City
                            && p.CountryCode == request.CountryCode
                            && p.IsActive,
                          cancellationToken);

            if (exists)
            {
                return OperationResult<PlaceDto>.Failure(
                    $"Place '{request.Name}' already exists in {request.City}");
            }

            if (!string.IsNullOrEmpty(request.AdditionalInfoJson))
            {
                try
                {
                    JsonDocument.Parse(request.AdditionalInfoJson);
                    _logger.LogInformation("✅ Valid JSON");
                }
                catch (JsonException)
                {
                    return OperationResult<PlaceDto>.Failure("Invalid JSON format in AdditionalInfo");
                }
            }

            // 2. Создаем место
            var place = new Place
            {
                Name = request.Name,
                Description = request.Description,
                CountryCode = request.CountryCode,
                City = request.City,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Category = request.Category,
                PlaceType = request.PlaceType,
                AdditionalInfo = request.AdditionalInfoJson,
                AverageRating = 0,
                ReviewsCount = 0,
                SavesCount = 0,
                ViewsCount = 0,
                //CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Places.Add(place);
            _logger.LogInformation($"🔄 Attempting to save place: {place.Name}");

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"✅ Place saved: {place.PlaceId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to save place. InnerException: {ex.InnerException?.Message}");

                // Возвращаем детальную ошибку
                return OperationResult<PlaceDto>.Failure(
                    $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }

            // 3. Загружаем изображения (если есть)
            var imageUrls = new List<string>();
            string? coverImageUrl = null;

            if (request.Images != null && request.Images.Any())
            {
                try
                {
                    var images = await _fileStorageService.UploadMultipleImagesAsync(
                        request.Images.ToArray(),
                        ImageEntityType.Place,
                        place.PlaceId,
                        request.CreatedBy,
                        cancellationToken);

                    if (images.Any())
                    {
                        _context.Images.AddRange(images);
                        await _context.SaveChangesAsync(cancellationToken);

                        imageUrls = images.Select(i => i.ImageUrl).ToList();
                        coverImageUrl = images.First().ImageUrl;

                        _logger.LogInformation($" Uploaded {images.Count} images for place {place.PlaceId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $" Failed to upload images for place {place.PlaceId}");
                }
            }

            if (request.Moods != null && request.Moods.Any())
            {
                var moods = request.Moods
                    .Distinct()
                    .Select(m => new PlaceMood
                    {
                        PlaceId = place.PlaceId,
                        Mood = m
                    });

                _context.PlaceMoods.AddRange(moods);
                await _context.SaveChangesAsync(cancellationToken);
            }

            PlaceAdditionalInfo? additionalInfoDto = null;

            if (!string.IsNullOrEmpty(request.AdditionalInfoJson))
            {
                try
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    additionalInfoDto = request.Category switch
                    {
                        PlaceCategory.Food => JsonSerializer.Deserialize<FoodPlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Accommodation => JsonSerializer.Deserialize<AccommodationPlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Culture => JsonSerializer.Deserialize<CulturePlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Nature => JsonSerializer.Deserialize<NaturePlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Entertainment => JsonSerializer.Deserialize<EntertainmentPlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Shopping => JsonSerializer.Deserialize<ShoppingPlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Transport => JsonSerializer.Deserialize<TransportPlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        PlaceCategory.Services => JsonSerializer.Deserialize<ServicePlaceInfo>(request.AdditionalInfoJson, jsonOptions),
                        _ => null
                    };
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize AdditionalInfo for response");
                }
            }
            // 4. Возвращаем DTO
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
                ImageUrls = imageUrls,
                CoverImageUrl = coverImageUrl,
                //CreatedBy = place.CreatedBy,
                CreatedAt = place.CreatedAt
            });
        }
    }
    }
