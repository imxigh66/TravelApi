using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.DTO.Users;
using Application.Posts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandler
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, OperationResult<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetPostByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
           
            var post = await _context.Posts
                    .Where(p => p.PostId == request.PostId)
                    .Select(p => new PostDto
                    {
                        PostId = p.PostId,
                        UserId = p.UserId,
                        PlaceId = p.PlaceId,
                        Content = p.Content,
                        Title = p.Title,
                        LikesCount=p.LikesCount,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                return OperationResult<PostDto>.Failure("Post not found.");

            }
            return OperationResult<PostDto>.Success(post);
        }
    }
}
