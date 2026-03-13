using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class EntertainmentPlaceInfo : PlaceAdditionalInfo
    {
        public string? PriceRange { get; set; }
        public decimal? AverageTicketPrice { get; set; }
        public string? Currency { get; set; }
        public string? OpeningHours { get; set; }
        public int? MinAge { get; set; }
        public int? AverageDurationMinutes { get; set; }
        public bool RequiresBooking { get; set; }
        public string? BookingUrl { get; set; }
        public string[]? Events { get; set; }
    }
}
