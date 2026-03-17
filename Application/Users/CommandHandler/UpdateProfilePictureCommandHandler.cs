using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Files;
using Application.Users.Command;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
           
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult<FileUploadResultDto>.Failure("User not found");
                }

                _logger.LogInformation($" Updating profile picture for user {user.UserId} ({user.Email})");

                
                using var imageStream = request.Image.OpenReadStream();

                if (!_imageProcessingService.IsValidImage(imageStream))
                {
                    return OperationResult<FileUploadResultDto>.Failure("Invalid image file");
                }

               
                imageStream.Position = 0;
                processedImageStream = await _imageProcessingService.CreateSquareImageAsync(imageStream, 400);

              
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var extension = Path.GetExtension(request.Image.FileName).ToLower();
                var fileName = $"avatar-{timestamp}{extension}";

               
                var folderPath = $"profiles/{user.UserId}";

              
                var oldImages = await _context.Images
                    .Where(i => i.EntityType == ImageEntityType.User
                             && i.EntityId == user.UserId
                             && i.IsCover
                             && i.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var oldImage in oldImages)
                {
                    oldImage.IsActive = false;
                    _logger.LogInformation($" Deactivated old profile image: {oldImage.ImageUrl}");

                    await _fileStorageService.DeleteFileAsync(oldImage.ImageUrl, cancellationToken);
                }

             
                var fileUrl = await _fileStorageService.UploadFileAsync(
                    processedImageStream,
                    fileName,
                    "image/jpeg",
                    folderPath,
                    cancellationToken);

               
                var imageEntity = new Domain.Entities.Image
                {
                    EntityType = ImageEntityType.User,
                    EntityId = user.UserId,
                    ImageUrl = fileUrl,
                    SortOrder = 1,
                    IsCover = true,
                    UploadedBy = user.UserId,
                    CreatedAt = DateTime.UtcNow,
                    MimeType = "image/jpeg",
                    OriginalFileName = request.Image.FileName,
                    FileSize = processedImageStream.Length,
                    Width = 400,
                    Height = 400,
                    IsActive = true
                };

                _context.Images.Add(imageEntity);

              
                user.ProfilePicture = fileUrl;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($" Profile picture updated successfully for user {user.UserId}");

      
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
               
                processedImageStream?.Dispose();
            }
        }
    }
}

