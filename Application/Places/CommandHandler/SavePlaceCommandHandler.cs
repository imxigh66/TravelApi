using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Places.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.CommandHandler
{
    public class SavePlaceCommandHandler : IRequestHandler<SavePlaceCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public SavePlaceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(SavePlaceCommand request, CancellationToken cancellationToken)
        {
            var alreadySaved = await _context.SavedPlaces
       .AnyAsync(x => x.UserId == request.UserId && x.PlaceId == request.PlaceId, cancellationToken);

            if (alreadySaved)
                return OperationResult<bool>.Failure("Place already saved");

            var placeExists = await _context.Places
                .AnyAsync(p => p.PlaceId == request.PlaceId && p.IsActive, cancellationToken);

            if (!placeExists)
                return OperationResult<bool>.Failure("Place not found");

            _context.SavedPlaces.Add(new SavedPlace
            {
                UserId = request.UserId,
                PlaceId = request.PlaceId,
                SavedAt = DateTime.UtcNow
            });

            await _context.Places
                .Where(p => p.PlaceId == request.PlaceId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.SavesCount, p => p.SavesCount + 1), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
