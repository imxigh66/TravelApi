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
    public class AddTripMemberCommand : IRequest<OperationResult<TripMemberDto>>
    {
        public int TripId { get; set; }
        public int OwnerId { get; set; }      // кто приглашает (должен быть Owner)
        public int TargetUserId { get; set; } // кого добавляем
        public TripMemberRole Role { get; set; } = TripMemberRole.Viewer;
    }
}
