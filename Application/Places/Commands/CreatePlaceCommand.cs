using Application.Common.Models;
using Application.DTO.Places;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Commands
{
    public class CreatePlaceCommand:IRequest<OperationResult<PlaceDto>>
    {
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

        // Контакты
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }

        // Дополнительная информация
        public string? AdditionalInfoJson { get; set; }

        // Изображения
        public List<IFormFile>? Images { get; set; }

        // Кто создает
        public int? CreatedBy { get; set; }
    }
}
