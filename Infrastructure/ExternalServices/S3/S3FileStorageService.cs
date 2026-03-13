using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.S3
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Options _options;
        private readonly ILogger<S3FileStorageService> _logger;
        public S3FileStorageService(IAmazonS3 s3Client, IOptions<S3Options> options,ILogger<S3FileStorageService> logger)
        {
            _s3Client = s3Client;
            _options = options.Value;
            _logger = logger;
        }
        public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileKey = ExtractFileKeyFromUrl(fileUrl);

                if (string.IsNullOrEmpty(fileKey))
                {
                    _logger.LogWarning($" Invalid file URL: {fileUrl}");
                    return false;
                }

                _logger.LogInformation($" Deleting file from S3: {fileKey}");

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = fileKey
                };

                await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

                _logger.LogInformation($" File deleted successfully: {fileKey}");

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $"❌ AWS S3 error deleting file: {fileUrl}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error deleting file: {fileUrl}");
                return false;
            }
        }

       

        public async Task<string> GetPreSignedUrlAsync(string fileKey, int expirationMinutes = 60)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.BucketName,
                    Key = fileKey,
                    Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
                };

                var url = await _s3Client.GetPreSignedURLAsync(request);

                _logger.LogInformation($" Generated pre-signed URL for: {fileKey}");

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Error generating pre-signed URL for: {fileKey}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(
    Stream fileStream,
    string fileName,
    string contentType,
    string folderPath,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var fileKey = $"{folderPath.TrimEnd('/')}/{fileName}";

                _logger.LogInformation($" Uploading file to S3: {fileKey}");

                // Копируем поток в MemoryStream, чтобы он не закрылся
                using var memoryStream = new MemoryStream();
                fileStream.Position = 0;
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = memoryStream,
                    Key = fileKey,
                    BucketName = _options.BucketName,
                    ContentType = contentType
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest, cancellationToken);

                var fileUrl = GetFileUrl(fileKey);

                _logger.LogInformation($" File uploaded successfully: {fileUrl}");

                return fileUrl;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $" AWS S3 error uploading file: {fileName}");
                throw new Exception($"Failed to upload file to S3: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Error uploading file: {fileName}");
                throw;
            }
        }

       


        public async Task<string> UploadImageAsync(
            IFormFile file,
            string folder = "posts",
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Валидация типа файла
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only images are allowed.");

            // Ограничение размера (5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 5MB");

            // Создать уникальное имя файла
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Получить MIME type
            var contentType = file.ContentType;

            // Вызвать базовый метод
            using var stream = file.OpenReadStream();
            return await UploadFileAsync(stream, fileName, contentType, folder, cancellationToken);
        }


        public async Task<Image> UploadImageWithMetadataAsync(
     IFormFile file,
     ImageEntityType entityType,
     int entityId,
     int sortOrder,
     bool isCover,
     int? uploadedBy = null,
     CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only images are allowed.");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 5MB");

            var folder = GetFolderByEntityType(entityType);
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Получаем размеры изображения
            int? width = null;
            int? height = null;

            using (var imageStream = file.OpenReadStream())
            {
                try
                {
                    using var loadedImage = await SixLabors.ImageSharp.Image.LoadAsync(imageStream, cancellationToken);
                    width = loadedImage.Width;
                    height = loadedImage.Height;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not read image dimensions");
                }
            }

            // Загружаем файл
            string imageUrl;
            using (var stream = file.OpenReadStream())
            {
                imageUrl = await UploadFileAsync(
                    stream,
                    fileName,
                    file.ContentType,
                    folder,
                    cancellationToken);
            }

            // Создаем объект ImageEntity
            var imageEntity = new Image
            {
                EntityType = entityType,
                EntityId = entityId,
                ImageUrl = imageUrl,
                SortOrder = sortOrder,
                IsCover = isCover,
                UploadedBy = uploadedBy,
                CreatedAt = DateTime.UtcNow,
                MimeType = file.ContentType,
                OriginalFileName = file.FileName,
                FileSize = file.Length,
                Width = width,
                Height = height,
                IsActive = true
            };

            _logger.LogInformation(
                $"✅ Image uploaded: {entityType}/{entityId} - {fileName} ({width}x{height}, {file.Length} bytes)");

            return imageEntity;
        }

        public async Task<List<Image>> UploadMultipleImagesAsync(
            IFormFile[] files,
            ImageEntityType entityType,
            int entityId,
            int? uploadedBy = null,
            CancellationToken cancellationToken = default)
        {
            if (files == null || files.Length == 0)
                return new List<Image>();

            var images = new List<Image>();
            var sortOrder = 1;

            foreach (var file in files)
            {
                try
                {
                    var image = await UploadImageWithMetadataAsync(
                        file,
                        entityType,
                        entityId,
                        sortOrder,
                        sortOrder == 1, // Первое фото = обложка
                        uploadedBy,
                        cancellationToken);

                    images.Add(image);
                    sortOrder++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to upload image {file.FileName}");
                    // Продолжаем загрузку остальных файлов
                }
            }

            _logger.LogInformation(
                $"✅ Uploaded {images.Count}/{files.Length} images for {entityType}/{entityId}");

            return images;
        }

        // ============ PRIVATE HELPERS ============

        private string? ExtractFileKeyFromUrl(string fileUrl)
        {
            try
            {
                if (_options.UseCloudFront && fileUrl.Contains(_options.CloudFrontUrl))
                {
                    var uri = new Uri(fileUrl);
                    return uri.AbsolutePath.TrimStart('/');
                }

                if (fileUrl.Contains(".s3.") && fileUrl.Contains(".amazonaws.com/"))
                {
                    var uri = new Uri(fileUrl);
                    return uri.AbsolutePath.TrimStart('/');
                }

                if (!fileUrl.StartsWith("http"))
                {
                    return fileUrl;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extracting file key from URL: {fileUrl}");
                return null;
            }
        }

        private string GetFileUrl(string fileKey)
        {
            if (_options.UseCloudFront && !string.IsNullOrEmpty(_options.CloudFrontUrl))
            {
                return $"{_options.CloudFrontUrl.TrimEnd('/')}/{fileKey}";
            }

            return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{fileKey}";
        }

        private static string GetFolderByEntityType(ImageEntityType entityType)
        {
            return entityType switch
            {
                ImageEntityType.Post => "posts",
                ImageEntityType.Place => "places",
                ImageEntityType.User => "profiles",
                ImageEntityType.Review => "reviews",
                ImageEntityType.Trip => "trips",
                _ => "uploads"
            };
        }
    }
}
