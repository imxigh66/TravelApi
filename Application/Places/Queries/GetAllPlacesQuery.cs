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
        public string? Category { get; set; }
        public string? City { get; set; }
        public MoodType? Mood { get; set; }
    }
}
