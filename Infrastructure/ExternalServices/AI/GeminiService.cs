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
    public class GeminiService 
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeminiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Gemini:ApiKey"]!;
        }

        public async Task<List<AiPlaceSuggestion>> SuggestPlacesAsync(
            string city, string userPrompt, List<string> alreadyAdded, CancellationToken cancellationToken = default)
        {
            var alreadyStr = alreadyAdded.Any()
                ? $"Уже в маршруте: {string.Join(", ", alreadyAdded)}. Не повторяй их."
                : "";

            var example = """[{"name":"название","category":"Food"}]""";
            var prompt = $"Город: {city}. {alreadyStr}\nЗапрос: \"{userPrompt}\".\nОтветь ТОЛЬКО JSON массивом такого вида: {example}\nМаксимум 6 мест.";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.0-pro:generateContent?key={_apiKey}";

            var body = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            var response = await _http.PostAsJsonAsync(url, body);
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== GEMINI RESPONSE ===");
            Console.WriteLine(json);
            Console.WriteLine("=======================");


            var doc = JsonDocument.Parse(json);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "[]";

            // убираем markdown если вдруг есть
            text = text.Trim().TrimStart('`');
            if (text.StartsWith("json")) text = text[4..];
            text = text.TrimEnd('`').Trim();

            return JsonSerializer.Deserialize<List<AiPlaceSuggestion>>(text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<AiPlaceSuggestion>();
        }
    }
}
