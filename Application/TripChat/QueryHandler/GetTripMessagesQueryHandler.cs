using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips;
using Application.TripChat.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TripChat.QueryHandler
{
    public class GetTripMessagesQueryHandler
        : IRequestHandler<GetTripMessagesQuery, PaginatedList<TripMessageDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTripMessagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<TripMessageDto>> Handle(
            GetTripMessagesQuery request, CancellationToken cancellationToken)
        {
            // Проверяем доступ: трип должен быть публичным
            // или принадлежать текущему пользователю
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip is null)
                return EmptyPage(request);

            var isMember = await _context.TripMembers
    .AnyAsync(m => m.TripId == request.TripId && m.UserId == request.UserId, cancellationToken);
            var hasAccess = trip.IsPublic || isMember;

            var query = _context.TripMessages
                .Include(m => m.Sender)
                .Where(m => m.TripId == request.TripId)
                .OrderByDescending(m => m.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new TripMessageDto
                {
                    TripMessageId = m.TripMessageId,
                    TripId = m.TripId,
                    SenderId = m.SenderId,
                    SenderUsername = m.Sender.Username,
                    SenderProfilePicture = m.Sender.ProfilePicture,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            items.Reverse(); // хронологический порядок

            return new PaginatedList<TripMessageDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize),
            };
        }

        private static PaginatedList<TripMessageDto> EmptyPage(GetTripMessagesQuery r) =>
            new() { Items = [], TotalCount = 0, PageNumber = r.PageNumber, PageSize = r.PageSize };
    }
}
