using Application.Common.Interfaces;
using Application.Notifications.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.CommandHandler
{
    public class MarkNotificationsReadCommandHandler
        : IRequestHandler<MarkNotificationsReadCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public MarkNotificationsReadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            MarkNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            await _context.Notifications
                .Where(n => n.RecipientId == request.UserId && !n.IsRead)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(n => n.IsRead, true),
                    cancellationToken);

            return true;
        }
    }
}
