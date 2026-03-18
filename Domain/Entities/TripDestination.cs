using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TripDestination
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string City { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public int SortOrder { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }

        // NAV
        public Trip Trip { get; set; } = null!;
    }
}
