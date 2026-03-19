using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.DTO.Trips;
using Application.Trips.Commands;
using Application.Users.Command;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, OperationResult<TripDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        public CreateTripCommandHandler(IApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<OperationResult<TripDto>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            

            var trip = new Domain.Entities.Trip
            {
                OwnerId = request.OwnerId,
                Title = request.Title,
                Description = request.Description,
                CountryCode = request.CountryCode,
                City = request.City,
                TripDate = request.TripDate,
                IsPublic = request.IsPublic,
                Status = TripStatus.Planned,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync(cancellationToken);

            var tripDto = new TripDto
            {
                TripId = trip.TripId,
                Title = trip.Title,
                Description = trip.Description,
                CountryCode = trip.CountryCode,
                City = trip.City,
                TripDate = trip.TripDate,
                IsPublic = trip.IsPublic,
                Status = trip.Status,
                CreatedAt = trip.CreatedAt,
            };

            await _mediator.Send(new SyncCountryFromTripCommand
            {
                UserId = request.OwnerId,
                CountryCode = request.CountryCode,
                City = request.City,
            }, cancellationToken);
            return OperationResult<TripDto>.Success(tripDto);


        }
    }
}
