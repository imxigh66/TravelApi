using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.Trips.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class UpdateTripNoteCommandHandler: IRequestHandler<UpdateTripNoteCommand, OperationResult<TripNoteDto>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateTripNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<TripNoteDto>> Handle(UpdateTripNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _context.TripNotes
                .Include(n => n.Trip)
                .FirstOrDefaultAsync(n => n.TripNoteId == request.NoteId, cancellationToken);

            if (note == null)
                return OperationResult<TripNoteDto>.Failure("Note not found.");

            if (note.Trip.OwnerId != request.UserId)
                return OperationResult<TripNoteDto>.Failure("Access denied.");

            note.Title = request.Title;
            note.Content = request.Content;
            note.UpdatedAt = DateTime.UtcNow;

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
