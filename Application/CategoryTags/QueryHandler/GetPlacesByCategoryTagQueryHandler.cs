using Application.CategoryTags.Queries;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.QueryHandler
{
    public class GetPlacesByCategoryTagQueryHandler : IRequestHandler<GetPlacesByCategoryTagQuery, PaginatedList<PlaceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPlacesByCategoryTagQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PlaceDto>> Handle(GetPlacesByCategoryTagQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CategoryTagLinks
                .AsNoTracking()
                .Where(l => l.CategoryTagId == request.CategoryTagId)
                .Select(l => l.Place)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PlaceDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    CountryCode = p.CountryCode,
                    City = p.City,
                    Address = p.Address,
                    PlaceType = p.PlaceType,
                    CreatedAt = p.CreatedAt,
                    CoverImageUrl = _context.Images
                .Where(i => i.EntityId == p.PlaceId && i.EntityType == ImageEntityType.Place)
                .OrderBy(i => i.CreatedAt)
                .Select(i => i.ImageUrl)
                .FirstOrDefault()
                });

            return await PaginatedList<PlaceDto>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );
        }
    }
}
