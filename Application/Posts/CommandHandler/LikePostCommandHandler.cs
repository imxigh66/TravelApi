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
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<LikePostCommandHandler> _logger;
        public LikePostCommandHandler(IApplicationDbContext context, ILogger<LikePostCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _context.Posts.FindAsync(new object[] { request.PostId }, cancellationToken);
            if(post==null)
            {
                _logger.LogWarning("Post with ID {PostId} not found for liking.", request.PostId);
                return false;
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l=>l.UserId==request.UserId && l.PostId==request.PostId, cancellationToken);

            if (existingLike != null)
            {
                _logger.LogInformation("User {UserId} already liked post {PostId}.", request.UserId, request.PostId);
                return true; 
            }

            var like = new Domain.Entities.Like
            {
                UserId = request.UserId,
                PostId = request.PostId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            post.LikesCount += 1;

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User {UserId} liked post {PostId}.", request.UserId, request.PostId);
            return true;
        }
    }
}
