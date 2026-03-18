using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class AddVisitedCountryRequest
    {
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateOnly? VisitedAt { get; set; }
        public string? Note { get; set; }
    }
}
