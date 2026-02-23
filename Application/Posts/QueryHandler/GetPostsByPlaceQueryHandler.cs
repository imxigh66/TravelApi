using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.Posts.Queries;
using Domain.Entities;
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
    public class GetPostsByPlaceQueryHandler : IRequestHandler<GetPostsByPlaceQuery, PaginatedList<PostDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPostsByPlaceQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<PostDto>> Handle(GetPostsByPlaceQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Posts
                 .AsNoTracking()
                 .Include(p => p.User)
                 .Include(p => p.Likes)
                 .Where(p => p.PlaceId == request.PlaceId)
                 .OrderByDescending(p => p.CreatedAt);

            var paginated=await PaginatedList<Post>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

            var postIds = paginated.Items.Select(p => p.PostId).ToList();

            var imagesByPost = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Post && postIds.Contains(i.EntityId) && i.IsActive)
                .GroupBy(i => i.EntityId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.ImageUrl).ToList(), cancellationToken);


            var dtos =paginated.Items.Select(p => new PostDto
            {
                PostId = p.PostId,
                UserId = p.UserId,
                Username = p.User.Username,
                UserProfilePicture = p.User.ProfilePicture,
                PlaceId = p.PlaceId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                LikesCount = p.Likes.Count,
                ImageUrls = imagesByPost.GetValueOrDefault(p.PostId) ?? new List<string>()
        }).ToList();

            return new PaginatedList<PostDto>(
            dtos, paginated.TotalCount,
            paginated.PageNumber, paginated.PageSize);
        }
    }
}
