using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TripPlace
    {
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int SortOrder { get; set; }
        public string? Notes { get; set; }

        // NAV
        public Trip Trip { get; set; } = null!;
        public Place Place { get; set; } = null!;
    }

}
