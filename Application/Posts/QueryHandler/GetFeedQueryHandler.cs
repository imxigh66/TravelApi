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
    public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, PaginatedList<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetFeedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<PostDto>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
        {
            var followingIds = await _context.UserFollows
       .Where(f => f.FollowerId == request.CurrentUserId)
       .Select(f => f.FollowingId)
       .ToListAsync(cancellationToken);

            var postQuery = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => followingIds.Contains(p.UserId))
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
                    CommentsCount = _context.Comments.Count(c => c.PostId == p.PostId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                });

            return await PaginatedList<PostDto>.CreateAsync(
                postQuery, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
