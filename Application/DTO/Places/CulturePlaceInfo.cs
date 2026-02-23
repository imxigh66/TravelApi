using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class CulturePlaceInfo : PlaceAdditionalInfo
    {
        public string? Period { get; set; }
        public string? Style { get; set; }
        public string? Architect { get; set; }
        public int? YearBuilt { get; set; }
        public decimal? TicketPrice { get; set; }
        public string? Currency { get; set; }
        public bool IsFree { get; set; }
        public string? OpeningHours { get; set; }
        public string? GuidedTours { get; set; }
        public string[]? Languages { get; set; }
        public int? DurationMinutes { get; set; }
    }
}
