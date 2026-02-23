using Application.Common.Models;
using Application.DTO.Places;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Queries
{
    public class GetAllPlacesQuery:IRequest<PaginatedList<PlaceDto>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int? CategoryTagId { get; set; }     // подборка модератора
        public PlaceCategory? Category { get; set; } // тип места
        public PlaceType? PlaceType { get; set; }    // подтип
        public MoodType? Mood { get; set; }          // настроение
        public string? City { get; set; }
        public string? CountryCode { get; set; }

        // Сортировка
        public string? SortBy { get; set; }
    }
}
