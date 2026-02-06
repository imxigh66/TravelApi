using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public string? Country { get; set; } = null!; // CHAR(2)
        public string? City { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }


        public bool EmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpires { get; set; }

        public TravelInterest? TravelInterest { get; set; }
        public TravelStyle? TravelStyle { get; set; }

        public AccountType AccountType { get; set; } = AccountType.Personal;

        // Business
      
        public BusinessType? BusinessType { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessWebsite { get; set; }
        public string? BusinessPhone { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // NAV
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();           // как владелец
        public ICollection<Place> PlacesCreated { get; set; } = new List<Place>();  // created_by
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
