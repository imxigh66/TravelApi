using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<UserVisitedCountry> UserVisitedCountries { get; }
        DbSet<SavedPlace> SavedPlaces { get; }
        DbSet<Place> Places { get; }
        DbSet<Trip> Trips { get; }
        DbSet<TripPlace> TripPlaces { get; }
        DbSet<Post> Posts { get; }
        DbSet<Comment> Comments { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Like> Likes { get; }
        DbSet<Image> Images { get; }
        DbSet<CategoryTag> CategoryTags { get; }
        DbSet<CategoryTagLink> CategoryTagLinks { get; }
        DbSet<PlaceMood> PlaceMoods { get; }

        DbSet<UserFollow> UserFollows { get;  }
        DbSet<TripNote> TripNotes { get; }
        DbSet<Review> Reviews { get; }
        DbSet<Budget> Budgets { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<TripDestination> TripDestinations { get; }
        DbSet<Conversation> Conversations { get; }
        DbSet<Message> Messages { get; }
        DbSet<TripMessage> TripMessages { get; }
        DbSet<TripMember> TripMembers { get; }
        DbSet<Notification> Notifications { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
