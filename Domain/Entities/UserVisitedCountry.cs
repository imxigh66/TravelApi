using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserVisitedCountry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CountryCode { get; set; } = null!; 
        public string? City { get; set; } = null!;
        public DateOnly? VisitedAt { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        // NAV
        public User User { get; set; } = null!;
    }
}
