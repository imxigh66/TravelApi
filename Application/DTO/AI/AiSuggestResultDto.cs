using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.AI
{
    public class AiSuggestResultDto
    {
        public List<AiPlaceSuggestionDto> Places { get; set; } = new();
        public string? Message { get; set; } 
    }
}
