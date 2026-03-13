using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class AccommodationPlaceInfo : PlaceAdditionalInfo
    {
        public int? RoomsCount { get; set; }
        public string? PriceRange { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Currency { get; set; }
        public string[]? RoomTypes { get; set; }
        public bool HasWifi { get; set; }
        public bool HasParking { get; set; }
        public bool HasBreakfast { get; set; }
        public bool PetFriendly { get; set; }
        public string? BookingUrl { get; set; }
    }
}
