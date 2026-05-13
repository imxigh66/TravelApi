
using Application.Comments.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Comment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.CommandHandler
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, OperationResult<CommentDto>>
    {
        public readonly IApplicationDbContext _context;
        public CreateCommentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _context.Posts.FindAsync(new object[] { request.PostId }, cancellationToken);

            if (post == null)
            {
                return OperationResult<CommentDto>.Failure("Post not found");
            }

           

            var comment = new Domain.Entities.Comment
            {
                UserId = request.UserId,
                PostId = request.PostId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            // Уведомление владельцу поста (не себе)
            if (post.UserId != request.UserId)
            {
                var actor = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
                _context.Notifications.Add(new Domain.Entities.Notification
                {
                    RecipientId = post.UserId,
                    ActorId = request.UserId,
                    Type = Domain.Enum.NotificationType.PostCommented,
                    Message = $"прокомментировал(а) ваш пост",
                    Link = $"/posts/{post.PostId}",
                    CreatedAt = DateTime.UtcNow,
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return OperationResult<CommentDto>.Success(new CommentDto
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Username = user?.Username ?? "",             
                UserProfilePicture = user?.ProfilePicture   
            });

        }
    }
}
