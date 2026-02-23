using Application.Common.Interfaces;
using Application.Posts.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.CommandHandler
{
    public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UnlikePostCommandHandler> _logger;

        public UnlikePostCommandHandler(IApplicationDbContext context, ILogger<UnlikePostCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == request.UserId && l.PostId == request.PostId, cancellationToken);
            if (like == null)
            {
                _logger.LogWarning("Like not found for User {UserId} and Post {PostId}.", request.UserId, request.PostId);
                return false;
            }
            var post = await _context.Posts.FindAsync(new object[] { request.PostId }, cancellationToken);
            if (post == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found for unliking.", request.PostId);
                return false;
            }

            _context.Likes.Remove(like);

            if (post.LikesCount > 0)
            {
                post.LikesCount -= 1;
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User {UserId} unliked post {PostId}.", request.UserId, request.PostId);
            return true;

        }
    }
}
