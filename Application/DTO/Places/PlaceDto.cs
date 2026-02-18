using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class PlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Локация
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Категоризация
        public PlaceCategory Category { get; set; }
        public PlaceType PlaceType { get; set; }

        

        // Рейтинг
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int SavesCount { get; set; }


        public object? AdditionalInfo { get; set; }
        // Изображения
        public List<string> ImageUrls { get; set; } = new();
        public string? CoverImageUrl { get; set; }

        // Метаданные
        //public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
