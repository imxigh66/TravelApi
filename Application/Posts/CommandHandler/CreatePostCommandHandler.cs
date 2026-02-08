using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Posts.Commands;
using AutoMapper;
using Domain.Entities;
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
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<CreatePostCommandHandler> _logger;
        public CreatePostCommandHandler(IApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<CreatePostCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
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

            var post = new Post
            {
                UserId = request.UserId,
                PlaceId = request.PlaceId,
                Title = request.Title,
                Content = request.Content,
                LikesCount = request.LikesCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            _context.Posts.Add(post);
            await _context.SaveChangesAsync(cancellationToken);

            if(request.Images != null && request.Images.Any())
            {
                var imageUrls = new List<string>();
                var sortOrder = 1;
                foreach (var imageFile in request.Images)
                {
                    try
                    {
                        var imageUrl = await _fileStorageService.UploadImageAsync(imageFile, "posts");

                        var postImage = new PostImage
                        {
                            PostId = post.PostId,
                            ImageUrl = imageUrl,
                            SortOrder = sortOrder++,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.PostImages.Add(postImage);
                        imageUrls.Add(imageUrl);
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex, $"Failed to upload image for post {post.PostId}");
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($" Uploaded {imageUrls.Count} images for post {post.PostId}");
            }
        

            var createdPost = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Place)
            .Include(p=>p.Images)
            .FirstAsync(p => p.PostId == post.PostId, cancellationToken);

            return OperationResult<PostDto>.Success(new PostDto
            {
                PostId = createdPost.PostId,
                UserId = createdPost.UserId,
                PlaceId = createdPost.PlaceId,
                Title = createdPost.Title,
                Content = createdPost.Content,
                ImageUrls = createdPost.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageUrl)
                    .ToList(),  
                LikesCount = createdPost.LikesCount,
                CreatedAt = createdPost.CreatedAt,
                UpdatedAt = createdPost.UpdatedAt,
            });

        }
    }
}
