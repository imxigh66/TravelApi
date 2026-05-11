using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Reviews;
using Application.Reviews.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.QueryHandlers
{
    public class GetPlaceReviewsQueryHandler
        : IRequestHandler<GetPlaceReviewsQuery, PaginatedList<ReviewDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPlaceReviewsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ReviewDto>> Handle(
            GetPlaceReviewsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.PlaceId == request.PlaceId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    PlaceId = r.PlaceId,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    UserProfilePicture = r.User.ProfilePicture,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<ReviewDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}
