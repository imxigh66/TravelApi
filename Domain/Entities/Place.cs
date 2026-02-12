using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Place
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Локация
        public string CountryCode { get; set; } = null!; // ISO 3166-1 alpha-2 (MD, US, FR)
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Категоризация
        public PlaceCategory Category { get; set; }
        public PlaceType PlaceType { get; set; }


        // Рейтинг и популярность
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int SavesCount { get; set; }
        public int ViewsCount { get; set; }

        // Дополнительная информация (JSON)
        public string? AdditionalInfo { get; set; } // JSON для специфичных полей

        // Владение бизнесом
        public int? BusinessOwnerId { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime? ClaimedAt { get; set; }

        // Метаданные
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public User? Creator { get; set; }
        public User? BusinessOwner { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<TripPlace> TripPlaces { get; set; } = new List<TripPlace>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}


