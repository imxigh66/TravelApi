using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class ServicePlaceInfo : PlaceAdditionalInfo
    {
        public string? PriceRange { get; set; }
        public string? OpeningHours { get; set; }
        public bool RequiresAppointment { get; set; }
        public string[]? ServicesOffered { get; set; }
    }
}
