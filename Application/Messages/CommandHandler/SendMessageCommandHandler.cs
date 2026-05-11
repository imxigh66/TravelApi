using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Messages;
using Application.Messages.Commands;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.CommandHandler
{
    public class SendMessageCommandHandler
        : IRequestHandler<SendMessageCommand, OperationResult<MessageDto>>
    {
        private readonly IApplicationDbContext _context;

        public SendMessageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<MessageDto>> Handle(
            SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conv = await _context.Conversations.FindAsync(
                new object[] { request.ConversationId }, cancellationToken);

            if (conv is null)
                return OperationResult<MessageDto>.Failure("Conversation not found.");

            if (conv.User1Id != request.SenderId && conv.User2Id != request.SenderId)
                return OperationResult<MessageDto>.Failure("Access denied.");

            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Messages.Add(message);

            conv.LastMessageText = request.Content.Length > 100
                ? request.Content[..100] + "…"
                : request.Content;
            conv.LastMessageAt = message.CreatedAt;

            await _context.SaveChangesAsync(cancellationToken);

            var sender = await _context.Users.FindAsync(
                new object[] { request.SenderId }, cancellationToken);

            return OperationResult<MessageDto>.Success(new MessageDto
            {
                MessageId = message.MessageId,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderUsername = sender?.Username ?? "",
                SenderProfilePicture = sender?.ProfilePicture,
                Content = message.Content,
                IsRead = false,
                CreatedAt = message.CreatedAt,
            });
        }
    }
}
