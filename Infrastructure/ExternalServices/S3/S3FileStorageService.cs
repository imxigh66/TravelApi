using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application.Common.Interfaces;
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

        private string? ExtractFileKeyFromUrl(string fileUrl)
        {
            try
            {
                // Если это CloudFront URL
                if (_options.UseCloudFront && fileUrl.Contains(_options.CloudFrontUrl))
                {
                    var uri = new Uri(fileUrl);
                    return uri.AbsolutePath.TrimStart('/');
                }

                // Если это S3 URL
                if (fileUrl.Contains(".s3.") && fileUrl.Contains(".amazonaws.com/"))
                {
                    var uri = new Uri(fileUrl);
                    return uri.AbsolutePath.TrimStart('/');
                }

                // Если передан просто ключ
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

        private string GetFileUrl(string fileKey)
        {
            if (_options.UseCloudFront && !string.IsNullOrEmpty(_options.CloudFrontUrl))
            {
                return $"{_options.CloudFrontUrl.TrimEnd('/')}/{fileKey}";
            }

            // Прямой S3 URL
            return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{fileKey}";
        }
    }
}
