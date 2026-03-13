using Application.Comments.Queries;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Comment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.QueryHandler
{
    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, PaginatedList<CommentDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetCommentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Comments
                 .AsNoTracking()
                 .Include(c => c.User)
                 .Where(c => c.PostId == request.PostId)
                 .OrderBy(c => c.CreatedAt)  
                 .Select(c => new CommentDto
                 {
                     CommentId = c.CommentId,
                     PostId = c.PostId,
                     UserId = c.UserId,
                     Username = c.User.Username,
                     UserProfilePicture = c.User.ProfilePicture,
                     Content = c.Content,
                     CreatedAt = c.CreatedAt
                 });

            return await PaginatedList<CommentDto>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        }
    }
}
