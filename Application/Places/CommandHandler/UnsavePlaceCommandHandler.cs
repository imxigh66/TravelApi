using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Places.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Places.CommandHandler
{
    public class UnsavePlaceCommandHandler : IRequestHandler<UnsavePlaceCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UnsavePlaceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(UnsavePlaceCommand request, CancellationToken cancellationToken)
        {
            var saved = await _context.SavedPlaces
       .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.PlaceId == request.PlaceId, cancellationToken);

            if (saved is null)
                return OperationResult<bool>.Failure("Place not saved");

            _context.SavedPlaces.Remove(saved);

            await _context.Places
                .Where(p => p.PlaceId == request.PlaceId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.SavesCount, p => p.SavesCount - 1), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
