using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Trip
    {
        public int TripId { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateOnly TripDate { get; set; }
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // NAV
        public User Owner { get; set; } = null!;
        public ICollection<TripPlace> TripPlaces { get; set; } = new List<TripPlace>();
        // удобный доступ ко множеству мест через TripPlaces можно собрать проекцией
    }

}
