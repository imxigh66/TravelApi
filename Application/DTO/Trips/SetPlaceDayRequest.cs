using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class SetPlaceDayRequest
    {
        public int? DayNumber { get; set; }
        public int? DestinationId { get; set; }
    }
}
