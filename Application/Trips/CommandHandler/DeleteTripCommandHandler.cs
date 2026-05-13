using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Trips.Commands;
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
    public class DeleteTripCommandHandler : IRequestHandler<DeleteTripCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteTripCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
        .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<bool>.Failure("Trip not found.");

            var member = await _context.TripMembers
    .FirstOrDefaultAsync(m => m.TripId == request.TripId && m.UserId == request.UserId, cancellationToken);

            if (member?.Role != TripMemberRole.Owner)
                return OperationResult <bool>.Failure("Access denied.");

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
