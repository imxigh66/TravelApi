using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Files;
using Application.Users.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, OperationResult<FileUploadResultDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UpdateBannerCommandHandler> _logger;

        public UpdateBannerCommandHandler(
            IApplicationDbContext context,
            IFileStorageService fileStorageService,
            ILogger<UpdateBannerCommandHandler> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }
        public async Task<OperationResult<FileUploadResultDto>> Handle(UpdateBannerCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
            if (user == null)
                return OperationResult<FileUploadResultDto>.Failure("User not found.");

            if (request.Image == null || request.Image.Length == 0)
                return OperationResult<FileUploadResultDto>.Failure("No image provided.");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(request.Image.ContentType.ToLower()))
                return OperationResult<FileUploadResultDto>.Failure("Invalid image format.");

            if (request.Image.Length > 8 * 1024 * 1024)
                return OperationResult<FileUploadResultDto>.Failure("File too large. Max 8MB.");

            using var inputStream = request.Image.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream, cancellationToken);

            // Resize to banner dimensions 1500x500
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(1500, 500),
                Mode = ResizeMode.Crop
            }));

            var processedStream = new MemoryStream();
            await image.SaveAsJpegAsync(processedStream, cancellationToken);
            processedStream.Position = 0;

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileName = $"banner-{timestamp}.jpg";
            var folderPath = $"profiles/{user.UserId}/banner";

            // Delete old banner file if exists
            if (!string.IsNullOrEmpty(user.BannerImage))
            {
                await _fileStorageService.DeleteFileAsync(user.BannerImage, cancellationToken);
            }

            var fileUrl = await _fileStorageService.UploadFileAsync(
                processedStream, fileName, "image/jpeg", folderPath, cancellationToken);

            user.BannerImage = fileUrl;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<FileUploadResultDto>.Success(new FileUploadResultDto
            {
                FileUrl = fileUrl,
                FileName = fileName,
                FileSize = processedStream.Length,
                ContentType = "image/jpeg",
                UploadedAt = DateTime.UtcNow
            });
        }
    }
}
