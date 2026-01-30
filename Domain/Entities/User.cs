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
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // NAV
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();           // как владелец
        public ICollection<Place> PlacesCreated { get; set; } = new List<Place>();  // created_by
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
