using Application.DTO.Posts;
using Application.Posts.Queries;
using Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandler
{
    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, OperationResult<List<PostDto>>>
    {
        private readonly TravelDbContext _context;
        public GetAllPostsQueryHandler(TravelDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<List<PostDto>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {

            var posts = _context.Posts.Select(post => new PostDto
            {
                PostId = post.PostId,
                UserId = post.UserId,
                PlaceId = post.PlaceId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CreatedAt = post.CreatedAt
            }).ToList();
            return new OperationResult<List<PostDto>>
            {
                IsSuccess = true,
                Data = posts
            };
        }
    }
}
