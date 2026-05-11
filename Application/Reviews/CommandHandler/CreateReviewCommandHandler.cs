using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Reviews;
using Application.Reviews.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.CommandHandler
{
    public class CreateReviewCommandHandler
        : IRequestHandler<CreateReviewCommand, OperationResult<ReviewDto>>
    {
        private readonly IApplicationDbContext _context;

        public CreateReviewCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<ReviewDto>> Handle(
            CreateReviewCommand request, CancellationToken cancellationToken)
        {
            // Валидация рейтинга
            if (request.Rating < 1 || request.Rating > 5)
                return OperationResult<ReviewDto>.Failure("Rating must be between 1 and 5.");

            // Проверяем, что место существует
            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId && p.IsActive, cancellationToken);

            if (place is null)
                return OperationResult<ReviewDto>.Failure("Place not found.");

            // Один отзыв на место от одного пользователя
            var existing = await _context.Reviews
                .FirstOrDefaultAsync(
                    r => r.PlaceId == request.PlaceId && r.UserId == request.UserId,
                    cancellationToken);

            if (existing is not null)
                return OperationResult<ReviewDto>.Failure("You have already reviewed this place.");

            var review = new Review
            {
                PlaceId = request.PlaceId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Reviews.Add(review);

            // Пересчитываем средний рейтинг и счётчик в Place
            var totalRating = await _context.Reviews
                .Where(r => r.PlaceId == request.PlaceId)
                .SumAsync(r => r.Rating, cancellationToken);

            var count = await _context.Reviews
                .CountAsync(r => r.PlaceId == request.PlaceId, cancellationToken);

            // +1 потому что новый отзыв ещё не сохранён
            place.ReviewsCount = count + 1;
            place.AverageRating = Math.Round((decimal)(totalRating + request.Rating) / (count + 1), 1);

            await _context.SaveChangesAsync(cancellationToken);

            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            return OperationResult<ReviewDto>.Success(new ReviewDto
            {
                ReviewId = review.ReviewId,
                PlaceId = review.PlaceId,
                UserId = review.UserId,
                Username = user?.Username ?? "",
                UserProfilePicture = user?.ProfilePicture,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
            });
        }
    }
}
