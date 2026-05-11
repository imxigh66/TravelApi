using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Reviews.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.CommandHandler
{
    public class DeleteReviewCommandHandler
        : IRequestHandler<DeleteReviewCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteReviewCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .FindAsync(new object[] { request.ReviewId }, cancellationToken);

            if (review is null)
                return OperationResult<bool>.Failure("Review not found.");

            if (review.UserId != request.UserId)
                return OperationResult<bool>.Failure("Access denied.");

            _context.Reviews.Remove(review);

            // Пересчёт рейтинга после удаления
            var place = await _context.Places.FindAsync(new object[] { review.PlaceId }, cancellationToken);
            if (place is not null)
            {
                var remaining = await _context.Reviews
                    .Where(r => r.PlaceId == review.PlaceId && r.ReviewId != review.ReviewId)
                    .ToListAsync(cancellationToken);

                place.ReviewsCount = remaining.Count;
                place.AverageRating = remaining.Count > 0
                    ? Math.Round((decimal)remaining.Sum(r => r.Rating) / remaining.Count, 1)
                    : 0;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
