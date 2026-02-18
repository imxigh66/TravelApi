using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class NaturePlaceInfo : PlaceAdditionalInfo
    {
        public string? Difficulty { get; set; }
        public int? LengthKm { get; set; }
        public int? DurationHours { get; set; }
        public int? ElevationMeters { get; set; }
        public string? BestSeason { get; set; }
        public bool RequiresPermit { get; set; }
        public decimal? EntryFee { get; set; }
        public string? Currency { get; set; }
        public string[]? Activities { get; set; }
        public bool HasParking { get; set; }
        public bool HasRestrooms { get; set; }
    }
}
