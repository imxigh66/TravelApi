using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace Infrastructure.ExternalServices.S3
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> _logger;

        public ImageProcessingService(ILogger<ImageProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task<Stream> ResizeImageAsync(Stream imageStream, int maxWidth, int maxHeight)
        {
            try
            {
                _logger.LogInformation($"🖼️ Resizing image to max {maxWidth}x{maxHeight}");

                imageStream.Position = 0;

                using var image = await Image.LoadAsync(imageStream);

                // Вычисляем новые размеры с сохранением пропорций
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                // Изменяем размер
                image.Mutate(x => x.Resize(newWidth, newHeight));

                // Сохраняем в новый поток
                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, new JpegEncoder { Quality = 85 });
                outputStream.Position = 0;

                _logger.LogInformation($"✅ Image resized to {newWidth}x{newHeight}");

                return outputStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error resizing image");
                throw;
            }
        }

        public async Task<Stream> CreateSquareImageAsync(Stream imageStream, int size)
        {
            try
            {
                _logger.LogInformation($"🔲 Creating square image {size}x{size}");

                imageStream.Position = 0;

                using var image = await Image.LoadAsync(imageStream);

                // Определяем минимальную сторону
                var minDimension = Math.Min(image.Width, image.Height);

                // Вычисляем координаты для обрезки по центру
                var cropX = (image.Width - minDimension) / 2;
                var cropY = (image.Height - minDimension) / 2;

                // Обрезаем до квадрата и изменяем размер
                image.Mutate(x => x
                    .Crop(new Rectangle(cropX, cropY, minDimension, minDimension))
                    .Resize(size, size));

                // Сохраняем в новый поток
                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, new JpegEncoder { Quality = 90 });
                outputStream.Position = 0;

                _logger.LogInformation($"✅ Square image created {size}x{size}");

                return outputStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating square image");
                throw;
            }
        }

        public bool IsValidImage(Stream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                Image.Identify(imageStream);
                imageStream.Position = 0;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(int width, int height)> GetImageDimensionsAsync(Stream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                var imageInfo = await Image.IdentifyAsync(imageStream);
                imageStream.Position = 0;

                return (imageInfo.Width, imageInfo.Height);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting image dimensions");
                throw;
            }
        }
    }
}
