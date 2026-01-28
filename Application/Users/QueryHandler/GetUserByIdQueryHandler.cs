using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.QueriesCommand;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueryHandler
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, OperationResult<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetUserByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return new OperationResult<UserDto>
                {
                    IsSuccess = false,
                    Error = "User not found."
                };
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return  OperationResult<UserDto>.Success(userDto);
        }
    }
}
