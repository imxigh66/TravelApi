using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Posts.Commands;
using AutoMapper;
using Domain.Entities;
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
        private readonly IMapper _mapper;
        public CreatePostCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<OperationResult<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == request.UserId, cancellationToken);
            if (!userExists)
            {
                return OperationResult<PostDto>.Failure("User not found");
            }

            if (request.PlaceId.HasValue)
            {
                var placeExists = await _context.Places
                    .AnyAsync(p => p.PlaceId == request.PlaceId.Value, cancellationToken);

                if (!placeExists)
                    return OperationResult<PostDto>.Failure($"Place with ID {request.PlaceId} not found");
            }

            var post = _mapper.Map<Post>(request);
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync(cancellationToken);

            var createdPost = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Place)
            .FirstAsync(p => p.PostId == post.PostId, cancellationToken);

            return OperationResult<PostDto>.Success(new PostDto
            {
                PostId = createdPost.PostId,
                UserId = createdPost.UserId,
                PlaceId = createdPost.PlaceId,
                Title = createdPost.Title,
                Content = createdPost.Content,
                ImageUrl = createdPost.ImageUrl,
                LikesCount = createdPost.LikesCount,
                CreatedAt = createdPost.CreatedAt,
                UpdatedAt = createdPost.UpdatedAt,
            });

        }
    }
}
