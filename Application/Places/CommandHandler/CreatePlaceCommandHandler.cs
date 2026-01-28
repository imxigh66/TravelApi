using Application.DTO.Auth;
using Application.DTO.Places;
using Application.Places.Commands;
using Domain.Entities;
using Infrastructure;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.CommandHandler
{
    public class CreatePlaceCommandHandler : IRequestHandler<CreatePlaceCommand, OperationResult<PlaceDto>>
    {
        private readonly TravelDbContext _context;
        public CreatePlaceCommandHandler(TravelDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PlaceDto>> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.Places.AnyAsync(p => p.Name == request.Name, cancellationToken);
            if (exists)
            {
                return new OperationResult<PlaceDto> { IsSuccess = false, Error = "This place already exists" };
            }


            var place = new Place
            {
                Name = request.Name,
                Description = request.Description,
                CountryCode = request.CountryCode,
                City = request.City,
                Address = request.Address,
                PlaceType = request.PlaceType,
                CreatedAt = DateTime.UtcNow
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync(cancellationToken);

            return new OperationResult<PlaceDto>
            {
                IsSuccess = true,
                Data = new PlaceDto
                {
                    PlaceId = place.PlaceId,
                    Name = place.Name,
                    Description = place.Description,
                    CountryCode = place.CountryCode,
                    City = place.City,
                    Address = place.Address,
                    PlaceType = place.PlaceType,
                    CreatedAt = place.CreatedAt
                }
            };
        }
    }
}
