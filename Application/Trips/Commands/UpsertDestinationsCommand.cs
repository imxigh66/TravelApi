using Application.Common.Models;
using Application.DTO.Trips.Destination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class UpsertDestinationsCommand : IRequest<OperationResult<List<TripDestinationDto>>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public List<DestinationItem> Destinations { get; set; } = new();
    }
}
