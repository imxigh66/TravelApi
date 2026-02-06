using Application.Auth.Register.Commands;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Register.CommandHandler
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {

        private readonly IApplicationDbContext _ctx;
        private readonly IEmailService _emailService;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        public ConfirmEmailCommandHandler(IApplicationDbContext ctx,IEmailService emailService,ILogger<ConfirmEmailCommandHandler> logger)
        {
            _ctx = ctx;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            // Ищем пользователя по токену
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.EmailConfirmationToken == request.Token, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"❌ Invalid confirmation token: {request.Token}");
                return false;
            }

            // Проверяем срок действия токена
            if (user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                _logger.LogWarning($"⏰ Expired confirmation token for user {user.Email}");
                return false;
            }

            // Подтверждаем email
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(cancellationToken);

            // Отправляем приветственное письмо
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send welcome email to {user.Email}");
                // Не возвращаем ошибку, т.к. email уже подтвержден
            }

            _logger.LogInformation($"✅ Email confirmed for user {user.Email}");
            return true;
        }
    }
}
