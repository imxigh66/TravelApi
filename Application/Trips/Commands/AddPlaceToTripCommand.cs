using Application.Common.Models;
using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class AddPlaceToTripCommand : IRequest<OperationResult<TripPlaceDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int PlaceId { get; set; }
        public string? Notes { get; set; }
    }
}
