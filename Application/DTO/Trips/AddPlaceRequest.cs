using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class AddPlaceRequest
    {
        public int PlaceId { get; set; }
        public string? Notes { get; set; }
    }
}
