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
                AdditionalInfo = request.AdditionalInfo,
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
