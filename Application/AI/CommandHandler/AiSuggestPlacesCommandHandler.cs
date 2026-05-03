using Application.AI.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.AI;
using Application.DTO.Places;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AI.CommandHandler
{
    public class AiSuggestPlacesCommandHandler
        : IRequestHandler<AiSuggestPlacesCommand, OperationResult<AiSuggestResultDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAiService _aiService;

        public AiSuggestPlacesCommandHandler(
            IApplicationDbContext context, IAiService aiService)
        {
            _context = context;
            _aiService=aiService;
        }

        public async Task<OperationResult<AiSuggestResultDto>> Handle(
            AiSuggestPlacesCommand request, CancellationToken cancellationToken)
        {

            var trip = await _context.Trips
     .Include(t => t.TripPlaces).ThenInclude(tp => tp.Place)
     .Include(t => t.Destinations)  
     .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);


            if (trip == null)
                return OperationResult<AiSuggestResultDto>.Failure("Trip not found");

            if (trip.OwnerId != request.UserId)
                return OperationResult<AiSuggestResultDto>.Failure("Access denied");

            var alreadyAdded = trip.TripPlaces
                .Select(tp => tp.Place.Name).ToList();

            var cities = trip.Destinations.Any()
    ? trip.Destinations.Select(d => d.City).ToList()
    : new List<string> { trip.City };

            var availablePlaces = await _context.Places
    .Where(p => p.IsActive && cities.Contains(p.City))
    .Select(p => new { p.Name, p.Category, p.City })
     .Distinct()
    .Take(50)
    .ToListAsync(cancellationToken);

            var availableStr = string.Join(", ", availablePlaces.Select(p => p.Name));

            Console.WriteLine($"Cities: {string.Join(", ", cities)}");
            Console.WriteLine($"Available places count: {availablePlaces.Count}");
            Console.WriteLine($"AvailableStr: {availableStr}");

            // передаём в сервис
            var suggestions = await _aiService.SuggestPlacesAsync(
                trip.City, request.Prompt, alreadyAdded, availableStr, cancellationToken);

            if (!suggestions.Any())
                return OperationResult<AiSuggestResultDto>.Success(new AiSuggestResultDto
                {
                    Places = new List<AiPlaceSuggestionDto>(),
                    Message = "AI не нашёл подходящих мест по вашему запросу"
                });


            var suggestedCategories = suggestions
    .Select(s => s.Category)
    .Distinct()
    .ToList();

            // парсим в enum
            var categoryEnums = suggestedCategories
                .Select(c => Enum.TryParse<PlaceCategory>(c, true, out var cat) ? cat : (PlaceCategory?)null)
                .Where(c => c.HasValue)
                .Select(c => c!.Value)
                .ToList();
            var names = suggestions.Select(s => s.Name.ToLower()).ToList();

            var places = await _context.Places
    .Where(p => p.IsActive && cities.Contains(p.City))
    .ToListAsync(cancellationToken);

            var filtered = places
    .Where(p => names.Any(n =>
        p.Name.ToLower() == n ||
        p.Name.ToLower().Contains(n) ||
        n.Contains(p.Name.ToLower())))
    .Take(6)
    .ToList();

            var placeIds = filtered.Select(p => p.PlaceId).ToList();

            var images = await _context.Images
    .Where(i => i.EntityType == ImageEntityType.Place
             && placeIds.Contains(i.EntityId)
             && i.IsCover)
    .ToListAsync(cancellationToken);



            var result = filtered.Select(p => new AiPlaceSuggestionDto
            {
                PlaceId = p.PlaceId,
                Name = p.Name,
                City = p.City,
                Address = p.Address,
                CoverImageUrl = images.FirstOrDefault(i => i.EntityId == p.PlaceId)?.ImageUrl,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Category = p.Category.ToString(),
                AverageRating = p.AverageRating,
                AlreadyInTrip = alreadyAdded.Contains(p.Name)
            }).ToList();
            Console.WriteLine($"Suggestions from AI: {string.Join(", ", names)}");
            Console.WriteLine($"Filtered count: {filtered.Count}");
            Console.WriteLine($"Result count: {result.Count}");
            return OperationResult<AiSuggestResultDto>.Success(new AiSuggestResultDto
            {
                Places = result,
                Message = null
            });
        }
    }
}
