using Application.Common.Models;
using Application.DTO.Trips;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class CreateTripCommand:IRequest<OperationResult<TripDto>>
    {
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public DateOnly TripDate { get; set; }
        public bool IsPublic { get; set; }
        public TripStatus Status { get; set; }
    }
}
