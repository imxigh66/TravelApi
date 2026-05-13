using Application.Common.Models;
using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripChat.Queries
{
    public class GetTripMessagesQuery : IRequest<PaginatedList<TripMessageDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
