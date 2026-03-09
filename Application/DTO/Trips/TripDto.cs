using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripDto
    {
        public int TripId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public DateOnly TripDate { get; set; }
        public bool IsPublic { get; set; }
        public TripStatus Status { get; set; }
        public int PlacesCount { get; set; }
        public string? CoverImageUrl { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
