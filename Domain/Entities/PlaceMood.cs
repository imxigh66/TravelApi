using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PlaceMood
    {
        public int PlaceId { get; set; }
        public MoodType Mood { get; set; }

        // NAV
        public Place Place { get; set; } = null!;
    }
}
