using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.Posts.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandler
{
    public class GetUserPostsQueryHandler : IRequestHandler<GetUserPostsQuery, PaginatedList<PostDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserPostsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PostDto>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == request.UserId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    UserProfilePicture = p.User.ProfilePicture,
                    PlaceId = p.PlaceId,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrls = _context.Images
                        .Where(i => i.EntityType == ImageEntityType.Post
                                 && i.EntityId == p.PostId
                                 && i.IsActive)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.ImageUrl)
                        .ToList(),
                    LikesCount = p.LikesCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                });

            return await PaginatedList<PostDto>.CreateAsync(
                query, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
