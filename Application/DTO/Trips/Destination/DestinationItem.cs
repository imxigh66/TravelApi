using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Destination
{
    public class DestinationItem
    {
        public int? Id { get; set; }          
        public string City { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public int SortOrder { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
