using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }

        /// <summary>Рейтинг от 1 до 5</summary>
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Place Place { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
