using Application.Common.Interfaces;
using Application.DTO.Users;
using Application.Posts.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandler
{
    public class GetPostLikesQueryHandler : IRequestHandler<GetPostLikesQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPostLikesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Likes
                .Where(l => l.PostId == request.PostId)
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new UserDto
                {
                    UserId = l.User.UserId,
                    Username = l.User.Username,
                    Name = l.User.Name,
                    Email = l.User.Email,
                    ProfilePicture = l.User.ProfilePicture,
                    Bio = l.User.Bio,
                    Country = l.User.Country,
                    City = l.User.City,
                    AccountType = l.User.AccountType,
                    CreatedAt = l.User.CreatedAt,
                    UpdatedAt = l.User.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return users;
        }
    }
}
