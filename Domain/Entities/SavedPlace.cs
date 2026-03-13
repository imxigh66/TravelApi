using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SavedPlace
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int PlaceId { get; set; }
        public Place Place { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
