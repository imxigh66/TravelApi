using Application.Common.Interfaces;
using Application.Notifications.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notifications.QueryHandler
{
    public class GetUnreadCountQueryHandler
         : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly IApplicationDbContext _context;

        public GetUnreadCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(
            GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientId == request.UserId && !n.IsRead, cancellationToken);
        }
    }
}
