using Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class CreatePlaceRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string Category { get; set; } = null!; // Строка, конвертируем в enum
        public string PlaceType { get; set; } = null!; // Строка, конвертируем в enum

        public string? AdditionalInfoJson { get; set; }


        public List<string>? Moods { get; set; }
        public List<IFormFile>? Images { get; set; } // ← Файлы изображений
    }
}
