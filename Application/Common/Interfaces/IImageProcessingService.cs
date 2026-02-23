using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IImageProcessingService
    {
        /// <summary>
        /// Изменяет размер изображения с сохранением пропорций
        /// </summary>
        /// <param name="imageStream">Поток изображения</param>
        /// <param name="maxWidth">Максимальная ширина</param>
        /// <param name="maxHeight">Максимальная высота</param>
        /// <returns>Поток измененного изображения</returns>
        Task<Stream> ResizeImageAsync(Stream imageStream, int maxWidth, int maxHeight);

        /// <summary>
        /// Создает квадратное изображение (для аватаров)
        /// </summary>
        /// <param name="imageStream">Поток изображения</param>
        /// <param name="size">Размер стороны квадрата</param>
        /// <returns>Поток обработанного изображения</returns>
        Task<Stream> CreateSquareImageAsync(Stream imageStream, int size);

        /// <summary>
        /// Проверяет, является ли файл изображением
        /// </summary>
        bool IsValidImage(Stream imageStream);

        /// <summary>
        /// Получает размеры изображения
        /// </summary>
        Task<(int width, int height)> GetImageDimensionsAsync(Stream imageStream);
    }
}
