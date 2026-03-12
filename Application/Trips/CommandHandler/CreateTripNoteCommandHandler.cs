using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
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
    public class CreateTripNoteCommandHandler:IRequestHandler<CreateTripNoteCommand, OperationResult<TripNoteDto>>
    {
        private readonly IApplicationDbContext _context;
        public CreateTripNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<TripNoteDto>> Handle(CreateTripNoteCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<TripNoteDto>.Failure("Trip not found.");

            if (trip.OwnerId != request.UserId)
                return OperationResult<TripNoteDto>.Failure("Access denied.");

            var note = new TripNote
            {
                TripId = request.TripId,
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TripNotes.Add(note);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<TripNoteDto>.Success(new TripNoteDto
            {
                TripNoteId = note.TripNoteId,
                Title = note.Title,
                Content = note.Content,
                UpdatedAt = note.UpdatedAt
            });
        }
    }
}
