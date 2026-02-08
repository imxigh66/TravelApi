using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Posts;
using Application.DTO.Users;
using Application.Posts.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandler
{
    public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, PaginatedList<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllPostsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {

            var postQuery = _context.Posts
        .AsNoTracking()
        .Include(p => p.Images)  
        .OrderByDescending(p => p.CreatedAt)
        .Select(p => new PostDto
        {
            PostId = p.PostId,
            UserId = p.UserId,
            PlaceId = p.PlaceId,
            Title = p.Title,
            Content = p.Content,
            ImageUrls = p.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ImageUrl)
                .ToList(),  
            LikesCount = p.LikesCount,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

            return await PaginatedList<PostDto>.CreateAsync(
                postQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );
        }
    }
}
