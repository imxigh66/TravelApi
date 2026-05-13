using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Messages;
using Application.Messages.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.QueryHandler
{
    public class GetMessagesQueryHandler
        : IRequestHandler<GetMessagesQuery, PaginatedList<MessageDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetMessagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<MessageDto>> Handle(
            GetMessagesQuery request, CancellationToken cancellationToken)
        {
            // Проверяем что пользователь участник диалога
            var inConversation = await _context.Conversations.AnyAsync(
                c => c.ConversationId == request.ConversationId &&
                     (c.User1Id == request.UserId || c.User2Id == request.UserId),
                cancellationToken);

            if (!inConversation)
                return new PaginatedList<MessageDto>
                {
                    Items = new List<MessageDto>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                };

            var query = _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == request.ConversationId)
                .OrderByDescending(m => m.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderUsername = m.Sender.Username,
                    SenderProfilePicture = m.Sender.ProfilePicture,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            // Помечаем входящие как прочитанные
            await _context.Messages
                .Where(m =>
                    m.ConversationId == request.ConversationId &&
                    m.SenderId != request.UserId &&
                    !m.IsRead)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(m => m.IsRead, true),
                    cancellationToken);

            // Возвращаем в хронологическом порядке
            items.Reverse();

            return new PaginatedList<MessageDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }
    }
}
