using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Destination
{
    public class UpsertDestinationsRequest
    {
        public List<DestinationItem> Destinations { get; set; } = new();
    }
}
