using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripMembers.Commands
{
    public class RemoveTripMemberCommand : IRequest<OperationResult<bool>>
    {
        public int TripId { get; set; }
        public int RequesterId { get; set; }  // кто удаляет
        public int TargetUserId { get; set; } // кого удаляем
    }
}
