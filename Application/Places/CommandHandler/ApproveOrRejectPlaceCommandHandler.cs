using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Commands;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.CommandHandler
{
    public class ApproveOrRejectPlaceCommandHandler : IRequestHandler<ApproveOrRejectPlaceCommand, OperationResult<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;

        public ApproveOrRejectPlaceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PlaceDto>> Handle(
           ApproveOrRejectPlaceCommand request,
           CancellationToken cancellationToken)
        {
            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.PlaceId == request.PlaceId, cancellationToken);

            if (place == null)
                return OperationResult<PlaceDto>.Failure("Place not found");

            if (place.Status != PlaceStatus.Pending)
                return OperationResult<PlaceDto>.Failure(
                    $"Place is already {place.Status}. Only pending places can be reviewed.");

            if (request.Approve)
            {
                place.Status = PlaceStatus.Approved;
                place.IsActive = true;
                place.RejectionReason = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.RejectionReason))
                    return OperationResult<PlaceDto>.Failure("Rejection reason is required");

                place.Status = PlaceStatus.Rejected;
                place.IsActive = false;
                place.RejectionReason = request.RejectionReason;
            }

            place.ReviewedBy = request.ModeratorId;
            place.ReviewedAt = DateTime.UtcNow;
            place.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

         
            var creator = place.CreatedBy.HasValue
                ? await _context.Users.FindAsync(place.CreatedBy.Value)
                : null;

            var images = await _context.Images
                .Where(i => i.EntityType == ImageEntityType.Place
                         && i.EntityId == place.PlaceId
                         && i.IsActive)
                .OrderBy(i => i.SortOrder)
                .ToListAsync(cancellationToken);

            return OperationResult<PlaceDto>.Success(new PlaceDto
            {
                PlaceId = place.PlaceId,
                Name = place.Name,
                Description = place.Description,
                CountryCode = place.CountryCode,
                City = place.City,
                Address = place.Address,
                Category = place.Category,
                PlaceType = place.PlaceType,
                AverageRating = place.AverageRating,
                ImageUrls = images.Select(i => i.ImageUrl).ToList(),
                CoverImageUrl = images.FirstOrDefault()?.ImageUrl,
                CreatedBy = place.CreatedBy,
                CreatorUsername = creator?.Username,
                CreatorProfilePicture = creator?.ProfilePicture,
                CreatedAt = place.CreatedAt,
                Status = place.Status,
                RejectionReason = place.RejectionReason,
                ReviewedAt = place.ReviewedAt,
            });
        }
    
}
}
