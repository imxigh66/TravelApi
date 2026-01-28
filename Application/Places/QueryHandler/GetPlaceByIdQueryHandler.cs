using Application.Common.Interfaces;
using Application.DTO.Places;
using Application.Places.Queries;
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
                        PlaceType = p.PlaceType,
                        CreatedAt = p.CreatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            if (place == null)
            {
                return new OperationResult<PlaceDto>
                {
                    IsSuccess = false,
                    Error = "Place not found."
                };
            }
            return new OperationResult<PlaceDto>
            {
                IsSuccess = true,
                Data = place
            };
        }
    }
}
