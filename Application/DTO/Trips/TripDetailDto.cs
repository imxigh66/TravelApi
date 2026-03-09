using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class TripDetailDto : TripDto
    {
        public int OwnerId { get; set; }
        public string OwnerUsername { get; set; }
        public string? OwnerProfilePicture { get; set; }
        public List<TripPlaceDto> Places { get; set; }
    }
}
