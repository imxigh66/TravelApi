using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.Posts.Queries;
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
        private readonly IApplicationDbContext _context;
        public GetAllPostsQueryHandler(IApplicationDbContext context)
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
            return  OperationResult<List<PostDto>>.Success(posts);
           
        }
    }
}
