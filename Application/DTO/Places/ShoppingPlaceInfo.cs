using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class ShoppingPlaceInfo : PlaceAdditionalInfo
    {
        public string? PriceRange { get; set; }
        public string? OpeningHours { get; set; }
        public string[]? Brands { get; set; }
        public string[]? Categories { get; set; }
        public bool AcceptsCreditCards { get; set; }
        public bool HasTaxFree { get; set; }
    }
}
