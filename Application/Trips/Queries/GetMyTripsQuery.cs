using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Queries
{
    public class GetMyTripsQuery : IRequest<List<TripDto>>
    {
        public int OwnerId { get; set; }
    }
}
