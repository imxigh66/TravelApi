using Application.Common.Interfaces;
using Application.Posts.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.CommandHandler
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        public DeletePostCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {

            var post = await _context.Posts
            .FirstOrDefaultAsync(
                p => p.PostId == request.PostId
                  && p.UserId == request.UserId,
                cancellationToken);
            if (post == null)
            {
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}
