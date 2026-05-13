using Application.Common.Models;
using Application.DTO.Trips;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripMembers.Commands
{
    public class UpdateTripMemberRoleCommand : IRequest<OperationResult<TripMemberDto>>
    {
        public int TripId { get; set; }
        public int OwnerId { get; set; }
        public int TargetUserId { get; set; }
        public TripMemberRole NewRole { get; set; }
    }
}
