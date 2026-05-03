using Application.Common.Models;
using Application.DTO.AI;
using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AI.Commands
{
    public class AiSuggestPlacesCommand : IRequest<OperationResult<AiSuggestResultDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public string Prompt { get; set; } = null!;
    }
}
