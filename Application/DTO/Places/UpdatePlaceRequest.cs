using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class UpdatePlaceRequest
    {
      
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Category { get; set; }
        public string? PlaceType { get; set; }
        public string? AdditionalInfoJson { get; set; }
        public List<string>? Moods { get; set; }

        // Изображения
        public List<IFormFile>? NewImages { get; set; }    // новые файлы
        public List<int>? DeleteImageIds { get; set; }     // id картинок на удаление
        public int? CoverImageId { get; set; }             // id новой обложки
    }
}
