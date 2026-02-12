using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.Places.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class GetPlaceByIdQueryHandler : IRequestHandler<GetPlaceByIdQuery, OperationResult<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetPlaceByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PlaceDto>> Handle(GetPlaceByIdQuery request, CancellationToken cancellationToken)
        {

            var place = await _context.Places
                    .Where(p => p.PlaceId == request.PlaceId)
                    .Select(p => new PlaceDto
                    {
                        PlaceId = p.PlaceId,
                        Name = p.Name,
                        Description = p.Description,
                        CountryCode = p.CountryCode,
                        City = p.City,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        Category = p.Category,
                        PlaceType = p.PlaceType,
                        AverageRating = p.AverageRating,
                        ReviewsCount = p.ReviewsCount,
                        SavesCount = p.SavesCount,
                        ImageUrls = _context.Images
                .Where(i => i.EntityType == ImageEntityType.Place
                         && i.EntityId == p.PlaceId
                         && i.IsActive)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ImageUrl)
                .ToList(),
                        CoverImageUrl = _context.Images
                        .Where(i => i.EntityType == ImageEntityType.Place
                                 && i.EntityId == p.PlaceId
                                 && i.IsCover
                                 && i.IsActive)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                        //CreatedBy = p.CreatedBy,
                        CreatedAt = p.CreatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            if (place == null)
            {
                return  OperationResult<PlaceDto>.Failure("Place not found.");
              
            }
            return  OperationResult<PlaceDto>.Success(place);

        }
    }
}
