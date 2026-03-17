using Application.Common.Models;
using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Commands
{
    public class ApproveOrRejectPlaceCommand : IRequest<OperationResult<PlaceDto>>
    {
        public int PlaceId { get; set; }
        public bool Approve { get; set; }          
        public string? RejectionReason { get; set; } 
        public int ModeratorId { get; set; }
    }
}
