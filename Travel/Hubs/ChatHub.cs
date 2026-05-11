using Application.Common.Interfaces;
using Application.DTO.Messages;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace TravelApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IApplicationDbContext _context;
        private readonly PresenceTracker _presence;


        public ChatHub(IApplicationDbContext context, PresenceTracker presence)
        {
            _context = context;
            _presence = presence;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            _presence.UserConnected(userId, Context.ConnectionId);

            // Оповещаем всех что юзер онлайн
            await Clients.All.SendAsync("UserOnline", userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            _presence.UserDisconnected(userId, Context.ConnectionId);

            // Оповещаем всех что юзер офлайн
            await Clients.All.SendAsync("UserOffline", userId);
            await base.OnDisconnectedAsync(exception);
        }
        // Клиент вызывает при открытии диалога
        public async Task JoinConversation(int conversationId)
        {
            var userId = GetUserId();
            var inConversation = await _context.Conversations.AnyAsync(c =>
                c.ConversationId == conversationId &&
                (c.User1Id == userId || c.User2Id == userId));
            if (!inConversation) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
        }

        // Клиент вызывает для отправки сообщения
        public async Task SendMessage(int conversationId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            var userId = GetUserId();

            var conv = await _context.Conversations.FindAsync(conversationId);
            if (conv is null) return;
            if (conv.User1Id != userId && conv.User2Id != userId) return;

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Messages.Add(message);

            conv.LastMessageText = content.Length > 100 ? content[..100] + "…" : content;
            conv.LastMessageAt = message.CreatedAt;

            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(userId);

            var dto = new MessageDto
            {
                MessageId = message.MessageId,
                ConversationId = conversationId,
                SenderId = userId,
                SenderUsername = sender?.Username ?? "",
                SenderProfilePicture = sender?.ProfilePicture,
                Content = content,
                IsRead = false,
                CreatedAt = message.CreatedAt,
            };

            // Шлём всем участникам группы (включая отправителя — для синхронизации вкладок)
            await Clients.Group($"conv_{conversationId}").SendAsync("ReceiveMessage", dto);
        }

        private int GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;
            return int.Parse(claim!);
        }
    }
}
