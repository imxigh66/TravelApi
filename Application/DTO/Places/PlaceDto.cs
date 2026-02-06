using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Places
{
    public class PlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string CountryCode { get; set; } = null!; // CHAR(2)
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public string? PlaceType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
