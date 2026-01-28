using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
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
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        public CreatePostCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            
            var post = new Domain.Entities.Post
            {
                UserId = request.UserId,
                PlaceId = request.PlaceId,
                Title = request.Title,
                Content = request.Content,
                ImageUrl = request.ImageUrl,
                LikesCount = request.LikesCount,
                CreatedAt = DateTime.UtcNow
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<PostDto>.Success(new PostDto
            {
                PostId = post.PostId,
                UserId = post.UserId,
                PlaceId = post.PlaceId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CreatedAt = post.CreatedAt
            });

        }
    }
}
