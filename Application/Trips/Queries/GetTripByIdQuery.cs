using Application.Common.Models;
using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Queries
{
    public class GetTripByIdQuery:IRequest<OperationResult<TripDetailDto>>
    {
        public int TripId { get; set; }
        public int? CurrentUserId { get; set; }
    }
}
