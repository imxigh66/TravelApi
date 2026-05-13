using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Messages;
using Application.Messages.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.CommandHandler
{
    public class StartConversationCommandHandler
        : IRequestHandler<StartConversationCommand, OperationResult<ConversationDto>>
    {
        private readonly IApplicationDbContext _context;

        public StartConversationCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<ConversationDto>> Handle(
            StartConversationCommand request, CancellationToken cancellationToken)
        {
            if (request.CurrentUserId == request.OtherUserId)
                return OperationResult<ConversationDto>.Failure("Cannot start a conversation with yourself.");

            var other = await _context.Users.FindAsync(
                new object[] { request.OtherUserId }, cancellationToken);

            if (other is null)
                return OperationResult<ConversationDto>.Failure("User not found.");

            // Ищем существующий диалог между двумя пользователями
            var existing = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == request.CurrentUserId && c.User2Id == request.OtherUserId) ||
                    (c.User1Id == request.OtherUserId && c.User2Id == request.CurrentUserId),
                    cancellationToken);

            if (existing is not null)
            {
                return OperationResult<ConversationDto>.Success(new ConversationDto
                {
                    ConversationId = existing.ConversationId,
                    OtherUserId = request.OtherUserId,
                    OtherUsername = other.Username,
                    OtherUserProfilePicture = other.ProfilePicture,
                    LastMessageText = existing.LastMessageText,
                    LastMessageAt = existing.LastMessageAt,
                    UnreadCount = 0,
                });
            }

            var conversation = new Conversation
            {
                User1Id = request.CurrentUserId,
                User2Id = request.OtherUserId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<ConversationDto>.Success(new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                OtherUserId = request.OtherUserId,
                OtherUsername = other.Username,
                OtherUserProfilePicture = other.ProfilePicture,
                UnreadCount = 0,
            });
        }
    }
}
