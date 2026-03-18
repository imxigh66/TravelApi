using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips.Destination;
using Application.Trips.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class UpsertDestinationsCommandHandler
       : IRequestHandler<UpsertDestinationsCommand, OperationResult<List<TripDestinationDto>>>
    {
        private readonly IApplicationDbContext _context;
        public UpsertDestinationsCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<OperationResult<List<TripDestinationDto>>> Handle(
            UpsertDestinationsCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<List<TripDestinationDto>>.Failure("Trip not found.");
            if (trip.OwnerId != request.UserId)
                return OperationResult<List<TripDestinationDto>>.Failure("Access denied.");

            var existing = await _context.TripDestinations
                .Where(d => d.TripId == request.TripId)
                .ToListAsync(cancellationToken);

           
            var incomingIds = request.Destinations
                .Where(d => d.Id.HasValue)
                .Select(d => d.Id!.Value)
                .ToHashSet();

            var toDelete = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
            _context.TripDestinations.RemoveRange(toDelete);

          
            foreach (var item in request.Destinations)
            {
                if (item.Id.HasValue)
                {
                    var dest = existing.FirstOrDefault(e => e.Id == item.Id.Value);
                    if (dest != null)
                    {
                        dest.City = item.City;
                        dest.CountryCode = item.CountryCode.ToUpper().Substring(0, 2);
                        dest.SortOrder = item.SortOrder;
                        dest.DateFrom = item.DateFrom;
                        dest.DateTo = item.DateTo;
                    }
                }
                else
                {
                    _context.TripDestinations.Add(new TripDestination
                    {
                        TripId = request.TripId,
                        City = item.City,
                        CountryCode = item.CountryCode.ToUpper().Substring(0, 2),
                        SortOrder = item.SortOrder,
                        DateFrom = item.DateFrom,
                        DateTo = item.DateTo,
                    });
                }
            }

        
            if (request.Destinations.Any())
            {
                var first = request.Destinations.OrderBy(d => d.SortOrder).First();
                trip.City = first.City;
                trip.CountryCode = first.CountryCode.ToUpper().Substring(0, 2);
                trip.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var result = await _context.TripDestinations
                .AsNoTracking()
                .Where(d => d.TripId == request.TripId)
                .OrderBy(d => d.SortOrder)
                .Select(d => new TripDestinationDto
                {
                    Id = d.Id,
                    TripId = d.TripId,
                    City = d.City,
                    CountryCode = d.CountryCode,
                    SortOrder = d.SortOrder,
                    DateFrom = d.DateFrom,
                    DateTo = d.DateTo,
                })
                .ToListAsync(cancellationToken);

            return OperationResult<List<TripDestinationDto>>.Success(result);
        }
    }
}
