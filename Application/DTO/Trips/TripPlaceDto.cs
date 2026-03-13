using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripPlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Notes { get; set; } 
        public int SortOrder { get; set; }
    }
}
