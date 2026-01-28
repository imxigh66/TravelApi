using Application.DTO.Places;
using Application.Places.Queries;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class GetAllPlacesQueryHandler : IRequestHandler<GetAllPlacesQuery, OperationResult<List<PlaceDto>>>
    {
        private readonly TravelDbContext _context;
        public GetAllPlacesQueryHandler(TravelDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<List<PlaceDto>>> Handle(GetAllPlacesQuery request, CancellationToken cancellationToken)
        {

            var places = await _context.Places
                .Select(p => new PlaceDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    CountryCode = p.CountryCode,
                    City = p.City,
                    Address = p.Address,
                    PlaceType = p.PlaceType,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);
            return new OperationResult<List<PlaceDto>>
            {
                IsSuccess = true,
                Data = places
            };
        }
    }
}
