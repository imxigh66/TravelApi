using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.AI
{
    public class AiPlaceSuggestionDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string City { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Category { get; set; } = null!;
        public decimal AverageRating { get; set; }
        public bool AlreadyInTrip { get; set; }  
    }
}
