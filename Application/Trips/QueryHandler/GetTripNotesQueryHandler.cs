using Application.Common.Interfaces;
using Application.DTO.Trips;
using Application.Trips.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.QueryHandler
{
    public class GetTripNotesQueryHandler: IRequestHandler<GetTripNotesQuery, List<TripNoteDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTripNotesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<TripNoteDto>> Handle(GetTripNotesQuery request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null || trip.OwnerId != request.UserId)
                return new List<TripNoteDto>();

            return await _context.TripNotes
                .Where(n => n.TripId == request.TripId)
                .OrderByDescending(n => n.UpdatedAt)
                .Select(n => new TripNoteDto
                {
                    TripNoteId = n.TripNoteId,
                    Title = n.Title,
                    Content = n.Content,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
