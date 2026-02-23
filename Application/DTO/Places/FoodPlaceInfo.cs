using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class FoodPlaceInfo : PlaceAdditionalInfo
    {
        [JsonPropertyName("cuisine")]
        public string? Cuisine { get; set; }

        [JsonPropertyName("dietOptions")]
        public string[]? DietOptions { get; set; }

        [JsonPropertyName("priceRange")]
        public string? PriceRange { get; set; }

        [JsonPropertyName("hasDelivery")]
        public bool HasDelivery { get; set; }

        [JsonPropertyName("hasTakeaway")]
        public bool HasTakeaway { get; set; }

        [JsonPropertyName("acceptsReservations")]
        public bool AcceptsReservations { get; set; }

        [JsonPropertyName("menuUrl")]
        public string? MenuUrl { get; set; }

        [JsonPropertyName("openingHours")]
        public string? OpeningHours { get; set; }
    }
}
