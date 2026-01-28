using Application.Auth.Login.Commands;
using Application.Common.Interfaces;
using Application.DTO.Auth;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Login.CommandHandler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<LoginResponse>>
    {
        private readonly IApplicationDbContext _context;
        public LoginCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                return new OperationResult<LoginResponse>
                {
                    IsSuccess = false,
                    Error = "Invalid email or password."
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new OperationResult<LoginResponse>
                {
                    IsSuccess = false,
                    Error = "Invalid email or password."
                };
            }

            return new OperationResult<LoginResponse>
            {
                IsSuccess = true,
                Data = new LoginResponse
                {
                    UserId = user.UserId
                }
            };

        }
    }
}
