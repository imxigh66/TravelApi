using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class SetPlaceDayCommand : IRequest<OperationResult<bool>>
    {
        public int TripId { get; set; }
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public int? DayNumber { get; set; }
        public int? DestinationId { get; set; }
    }
}
