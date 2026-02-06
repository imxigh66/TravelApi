using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class TravelDbContext : DbContext, IApplicationDbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<TripPlace> TripPlaces => Set<TripPlace>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // USERS
            b.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.UserId);
                e.Property(x => x.Username).HasMaxLength(50).IsRequired();
                e.Property(x => x.Email).HasMaxLength(255).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100);
                e.Property(x => x.Country).HasMaxLength(2);
                e.Property(x => x.City).HasMaxLength(100);
                e.Property(x => x.Bio).HasColumnType("nvarchar(max)");
                e.Property(x => x.ProfilePicture).HasMaxLength(500);
                // enums → string
                e.Property(x => x.TravelInterest)
                    .HasConversion<string>(); 

                e.Property(x => x.TravelStyle)
                    .HasConversion<string>();

                e.Property(x => x.AccountType)
                    .HasConversion<string>()
                    .IsRequired();

                // Business
                e.Property(x => x.BusinessType)
                    .HasConversion<string>();

                e.Property(x => x.BusinessAddress)
                    .HasMaxLength(255);

                e.Property(x => x.BusinessWebsite)
                    .HasMaxLength(255);

                e.Property(x => x.BusinessPhone)
                    .HasMaxLength(30);

                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
            });

            // PLACE
            b.Entity<Place>(e =>
            {
                e.ToTable("place");
                e.HasKey(x => x.PlaceId);
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.CountryCode).HasMaxLength(2).IsRequired();
                e.Property(x => x.City).HasMaxLength(100).IsRequired();
                e.Property(x => x.Address).HasMaxLength(255);
                e.Property(x => x.PlaceType).HasMaxLength(50);
                e.HasIndex(x => new { x.CountryCode, x.City }).HasDatabaseName("ix_place_location");
                e.HasIndex(x => x.PlaceType).HasDatabaseName("ix_place_type");

                e.HasOne(x => x.Creator)
                 .WithMany(u => u.PlacesCreated)
                 .HasForeignKey(x => x.CreatedBy)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // TRIP
            b.Entity<Trip>(e =>
            {
                e.ToTable("trip");
                e.HasKey(x => x.TripId);
                e.Property(x => x.Title).HasMaxLength(150).IsRequired();
                e.Property(x => x.CountryCode).HasMaxLength(2).IsRequired();
                e.Property(x => x.City).HasMaxLength(100).IsRequired();
                e.Property(x => x.TripDate).HasColumnType("date"); // SQL Server: type date

                e.HasIndex(x => x.OwnerId).HasDatabaseName("ix_trip_owner");
                e.HasIndex(x => new { x.CountryCode, x.City }).HasDatabaseName("ix_trip_location");
                e.HasIndex(x => new { x.IsPublic, x.CreatedAt }).HasDatabaseName("ix_trip_public");

                e.HasOne(x => x.Owner)
                 .WithMany(u => u.Trips)
                 .HasForeignKey(x => x.OwnerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // TRIP_PLACE (many-to-many Trip ↔ Place)
            b.Entity<TripPlace>(e =>
            {
                e.ToTable("trip_place");
                e.HasKey(x => new { x.TripId, x.PlaceId });
                e.Property(x => x.Notes).HasColumnType("nvarchar(max)");
                e.HasIndex(x => new { x.TripId, x.SortOrder }).HasDatabaseName("ix_trip_place_sort");

                e.HasOne(x => x.Trip)
                 .WithMany(t => t.TripPlaces)
                 .HasForeignKey(x => x.TripId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Place)
                 .WithMany(p => p.TripPlaces)
                 .HasForeignKey(x => x.PlaceId)
                 .OnDelete(DeleteBehavior.Restrict); // как в T-SQL: NO ACTION
            });

            // POST
            b.Entity<Post>(e =>
            {
                e.ToTable("post");
                e.HasKey(x => x.PostId);
                e.Property(x => x.Title).HasMaxLength(200);
                e.Property(x => x.ImageUrl).HasMaxLength(500);
                e.HasIndex(x => new { x.UserId, x.CreatedAt }).HasDatabaseName("ix_post_user_created");
                e.HasIndex(x => x.PlaceId).HasDatabaseName("ix_post_place");
                e.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_post_recent");

                e.HasOne(x => x.User)
                 .WithMany(u => u.Posts)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // как в T-SQL

                e.HasOne(x => x.Place)
                 .WithMany(p => p.Posts)
                 .HasForeignKey(x => x.PlaceId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // COMMENT
            b.Entity<Comment>(e =>
            {
                e.ToTable("comment");
                e.HasKey(x => x.CommentId);
                e.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
                e.HasIndex(x => new { x.PostId, x.CreatedAt }).HasDatabaseName("ix_comment_post_created");
                e.HasIndex(x => x.UserId).HasDatabaseName("ix_comment_user");

                e.HasOne(x => x.Post)
                 .WithMany(p => p.Comments)
                 .HasForeignKey(x => x.PostId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.User)
                 .WithMany() 
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
            });


            b.Entity<RefreshToken>(e =>
            {
                e.ToTable("refresh_token");
                e.HasKey(x => x.RefreshTokenId);
                e.Property(x => x.Token).HasMaxLength(500).IsRequired();
                e.HasIndex(x => x.Token).IsUnique();
                e.HasIndex(x => new { x.UserId, x.ExpiresAt });

                e.HasOne(x => x.User)
                 .WithMany(u => u.RefreshTokens)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }

}
