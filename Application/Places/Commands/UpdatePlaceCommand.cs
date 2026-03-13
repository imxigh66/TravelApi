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
    public class UpdatePlaceCommand : IRequest<OperationResult<PlaceDto>>
    {
        public int PlaceId { get; set; }

        // Все поля nullable — если null, не обновляем
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public PlaceCategory? Category { get; set; }
        public PlaceType? PlaceType { get; set; }
        public string? AdditionalInfoJson { get; set; }
        public List<MoodType>? Moods { get; set; }

        // Изображения
        public List<IFormFile>? NewImages { get; set; }       // добавить новые
        public List<int>? DeleteImageIds { get; set; }        // удалить конкретные по id
        public int? CoverImageId { get; set; }                // сменить обложку

        public int? UpdatedBy { get; set; }
    }
}
