using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
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
    public class DeleteTripNoteCommandHandler: IRequestHandler<DeleteTripNoteCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteTripNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(DeleteTripNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _context.TripNotes
                .Include(n => n.Trip)
                .FirstOrDefaultAsync(n => n.TripNoteId == request.NoteId, cancellationToken);

            if (note == null)
                return OperationResult<bool>.Failure("Note not found.");

            var member = await _context.TripMembers
    .FirstOrDefaultAsync( m=>m.UserId == request.UserId, cancellationToken);

            if (member is null || member.Role == TripMemberRole.Viewer)
                return OperationResult<bool>.Failure("Access denied.");

            _context.TripNotes.Remove(note);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
