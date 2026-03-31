using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.AI
{
    public class GroqService : IAiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GroqService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Groq:ApiKey"]!;
        }

        public async Task<List<AiPlaceSuggestion>> SuggestPlacesAsync(
            string city, string userPrompt, List<string> alreadyAdded, string availablePlaces,
            CancellationToken cancellationToken = default)
        {
            var alreadyStr = alreadyAdded.Any()
                ? $"Уже в маршруте: {string.Join(", ", alreadyAdded)}. Не повторяй их."
                : "";

            var example = """[{"name":"название","category":"Food"}]""";
            var prompt = $"City: {city}. {alreadyStr}\n" +
             $"Available places: {availablePlaces}.\n" +
             $"User request: \"{userPrompt}\".\n" +
             $"Select places from the available list that match the request.\n" +
             $"Return ONLY their EXACT names from the list above as JSON: {example}\n" +
             $"Maximum 6 places. Use exact names as provided.";

            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var body = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0.7
            };

            request.Content = JsonContent.Create(body);

            var response = await _http.SendAsync(request, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            Console.WriteLine("=== GROQ RESPONSE ===");
            Console.WriteLine(json);

            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("error", out var error))
                throw new Exception($"Groq error: {error.GetProperty("message").GetString()}");

           

            var text = doc.RootElement
    .GetProperty("choices")[0]
    .GetProperty("message")
    .GetProperty("content")
    .GetString() ?? "[]";

          
            var start = text.IndexOf('[');
            var end = text.LastIndexOf(']');

            if (start == -1 || end == -1 || end < start)
                return new List<AiPlaceSuggestion>();

            text = text[start..(end + 1)];

            return JsonSerializer.Deserialize<List<AiPlaceSuggestion>>(text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<AiPlaceSuggestion>();
        }
    }
}
