using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Place
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string CountryCode { get; set; } = null!; // CHAR(2)
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public string? PlaceType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // NAV
        public User? Creator { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<TripPlace> TripPlaces { get; set; } = new List<TripPlace>();
    }

}
