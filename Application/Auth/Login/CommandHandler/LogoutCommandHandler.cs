using Application.Auth.Login.Commands;
using Application.Auth.Register.CommandHandler;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Login.CommandHandler
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<LogoutCommandHandler> _logger;
        public LogoutCommandHandler(IApplicationDbContext context,ILogger<LogoutCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    rt => rt.Token == request.RefreshToken && !rt.IsRevoked,
                    cancellationToken);

            if (refreshToken == null)
            {
              
                return true;
            }

            refreshToken.IsRevoked = true;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Logout is succes");

            return true;
        }
    }
}
