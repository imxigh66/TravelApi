using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Files;
using Application.Users.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, OperationResult<FileUploadResultDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ILogger<UpdateProfilePictureCommandHandler> _logger;

        public UpdateProfilePictureCommandHandler(
            IApplicationDbContext context,
            IFileStorageService fileStorageService,
            IImageProcessingService imageProcessingService,
            ILogger<UpdateProfilePictureCommandHandler> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _imageProcessingService = imageProcessingService;
            _logger = logger;
        }

        public async Task<OperationResult<FileUploadResultDto>> Handle(
            UpdateProfilePictureCommand request,
            CancellationToken cancellationToken)
        {
            Stream? processedImageStream = null;

            try
            {
                // 1. Находим пользователя
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult<FileUploadResultDto>.Failure("User not found");
                }

                _logger.LogInformation($" Updating profile picture for user {user.UserId} ({user.Email})");

                // 2. Валидируем изображение
                using var imageStream = request.Image.OpenReadStream();

                if (!_imageProcessingService.IsValidImage(imageStream))
                {
                    return OperationResult<FileUploadResultDto>.Failure("Invalid image file");
                }

                // 3. Обрабатываем изображение (создаем квадратное 400x400)
                imageStream.Position = 0;
                processedImageStream = await _imageProcessingService.CreateSquareImageAsync(imageStream, 400);

                // 4. Генерируем уникальное имя файла
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var extension = Path.GetExtension(request.Image.FileName).ToLower();
                var fileName = $"avatar-{timestamp}{extension}";

                // 5. Формируем путь к папке пользователя
                var folderPath = $"profiles/{user.UserId}";

                // 6. Удаляем старый аватар, если существует
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    _logger.LogInformation($" Deleting old profile picture: {user.ProfilePicture}");
                    await _fileStorageService.DeleteFileAsync(user.ProfilePicture, cancellationToken);
                }

                // 7. Загружаем новое изображение в S3
                var fileUrl = await _fileStorageService.UploadFileAsync(
                    processedImageStream,
                    fileName,
                    "image/jpeg",
                    folderPath,
                    cancellationToken);

                // 8. Обновляем URL в базе данных
                user.ProfilePicture = fileUrl;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($" Profile picture updated successfully for user {user.UserId}");

                // 9. Возвращаем результат
                var result = new FileUploadResultDto
                {
                    FileUrl = fileUrl,
                    FileName = fileName,
                    FileSize = processedImageStream.Length,
                    ContentType = "image/jpeg",
                    UploadedAt = DateTime.UtcNow
                };

                return OperationResult<FileUploadResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Error updating profile picture for user {request.UserId}");
                return OperationResult<FileUploadResultDto>.Failure($"Failed to update profile picture: {ex.Message}");
            }
            finally
            {
                // Освобождаем ресурсы
                processedImageStream?.Dispose();
            }
        }
    }
}

