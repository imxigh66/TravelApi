using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class TransportPlaceInfo : PlaceAdditionalInfo
    {
        public string[]? Services { get; set; }
        public string? Schedule { get; set; }
        public bool Has24HourService { get; set; }
    }
}
