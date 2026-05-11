using Application.Common.Interfaces;
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
    public class GetConversationsQueryHandler
        : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetConversationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConversationDto>> Handle(
            GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var conversations = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Where(c => c.User1Id == request.UserId || c.User2Id == request.UserId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = new List<ConversationDto>();

            foreach (var c in conversations)
            {
                var isUser1 = c.User1Id == request.UserId;
                var other = isUser1 ? c.User2 : c.User1;
                var otherId = isUser1 ? c.User2Id : c.User1Id;

                var unread = await _context.Messages
                    .CountAsync(m =>
                        m.ConversationId == c.ConversationId &&
                        m.SenderId != request.UserId &&
                        !m.IsRead,
                        cancellationToken);

                result.Add(new ConversationDto
                {
                    ConversationId = c.ConversationId,
                    OtherUserId = otherId,
                    OtherUsername = other.Username,
                    OtherUserProfilePicture = other.ProfilePicture,
                    LastMessageText = c.LastMessageText,
                    LastMessageAt = c.LastMessageAt,
                    UnreadCount = unread,
                });
            }

            return result;
        }
    }
}
