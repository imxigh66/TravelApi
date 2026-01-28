using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueryHandler
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, OperationResult<List<UserDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().Select(user => new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToListAsync(cancellationToken);

            return  OperationResult<List<UserDto>>.Success(users);
           
        }
    }
}
