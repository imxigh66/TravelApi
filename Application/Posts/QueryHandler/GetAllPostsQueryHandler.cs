using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.Posts.Queries;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.QueryHandler
{
    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, PaginatedList<PostDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPostsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
           
            var query = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .AsQueryable();

            if (request.UserId.HasValue)
            {
               
                var userPosts = await query
                    .Where(p => p.UserId == request.UserId.Value)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new
                    {
                        p.PostId,
                        p.UserId,
                        p.PlaceId,
                        p.Title,
                        p.Content,
                        p.LikesCount,
                        p.CreatedAt,
                        p.UpdatedAt,
                        p.User.Username,
                        p.User.ProfilePicture
                    })
                    .ToListAsync(cancellationToken);

                var userPostIds = userPosts.Select(p => p.PostId).ToList();

                var commentCounts = await _context.Comments
                    .Where(c => userPostIds.Contains(c.PostId))
                    .GroupBy(c => c.PostId)
                    .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);

                var userImages = await _context.Images
                    .AsNoTracking()
                    .Where(i => i.EntityType == ImageEntityType.Post
                             && userPostIds.Contains(i.EntityId)
                             && i.IsActive)
                    .GroupBy(i => i.EntityId)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
                        cancellationToken);

                var userDtos = userPosts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    UserId = p.UserId,
                    Username = p.Username,
                    UserProfilePicture = p.ProfilePicture,
                    PlaceId = p.PlaceId,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrls = userImages.GetValueOrDefault(p.PostId) ?? new List<string>(),
                    LikesCount = p.LikesCount,
                    CommentsCount = commentCounts.GetValueOrDefault(p.PostId, 0),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                var userPaged = userDtos
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PaginatedList<PostDto>(userPaged, userDtos.Count, request.PageNumber, request.PageSize);
            }

            
            var allPosts = await query
                .Select(p => new
                {
                    p.PostId,
                    p.UserId,
                    p.PlaceId,
                    p.Title,
                    p.Content,
                    p.LikesCount,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.User.Username,
                    p.User.ProfilePicture
                })
                .ToListAsync(cancellationToken);

            //  Decay-сортировка в памяти:
            //    score = LikesCount / (hoursOld + 2)^1.5
            //    Свежий пост с 10 лайками > старый с 100
            var now = DateTime.UtcNow;

            var sorted = allPosts
                .OrderByDescending(p =>
                {
                    var hoursOld = (now - p.CreatedAt).TotalHours;
                    return p.LikesCount / Math.Pow(hoursOld + 2, 1.5);
                })
                .ToList();

            var totalCount = sorted.Count;

            var pagedPosts = sorted
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

          
            var postIds = pagedPosts.Select(p => p.PostId).ToList();

            var imagesByPost = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ImageEntityType.Post
                         && postIds.Contains(i.EntityId)
                         && i.IsActive)
                .GroupBy(i => i.EntityId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
                    cancellationToken);

           
            var dtos = pagedPosts.Select(p => new PostDto
            {
                PostId = p.PostId,
                UserId = p.UserId,
                Username = p.Username,
                UserProfilePicture = p.ProfilePicture,
                PlaceId = p.PlaceId,
                Title = p.Title,
                Content = p.Content,
                ImageUrls = imagesByPost.GetValueOrDefault(p.PostId) ?? new List<string>(),
                LikesCount = p.LikesCount,
                CommentsCount = _context.Comments.Count(c => c.PostId == p.PostId),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return new PaginatedList<PostDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}