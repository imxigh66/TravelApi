using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips
{
    public class CreateTripRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateOnly TripDate { get; set; }
        public bool IsPublic { get; set; } = true;
        public TripStatus Status { get; set; } = TripStatus.Planned;
    }
}
