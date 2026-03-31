using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Trips;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AI.Commands
{
    public class AiSuggestPlacesCommand : IRequest<OperationResult<List<AiPlaceSuggestionDto>>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public string Prompt { get; set; } = null!;
    }
}
