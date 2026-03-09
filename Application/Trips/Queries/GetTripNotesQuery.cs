using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Queries
{
    public class GetTripNotesQuery : IRequest<List<TripNoteDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
    }
}
