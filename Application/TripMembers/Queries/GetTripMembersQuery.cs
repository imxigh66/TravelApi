using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripMembers.Queries
{
    public class GetTripMembersQuery : IRequest<List<TripMemberDto>>
    {
        public int TripId { get; set; }
        public int RequesterId { get; set; }
    }
}
