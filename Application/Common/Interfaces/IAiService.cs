using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAiService
    {
        Task<List<AiPlaceSuggestion>> SuggestPlacesAsync(
            string city,
            string userPrompt,
            List<string> alreadyAdded,
            string availablePlaces,
            CancellationToken cancellationToken = default);
    }
}
