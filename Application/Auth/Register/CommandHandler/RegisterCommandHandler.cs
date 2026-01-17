using Application.Auth.Register.Commands;
using Application.DTO.Auth;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Register.CommandHandler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<RegisterResponse>>
    {
        private readonly TravelDbContext _ctx;
        public RegisterCommandHandler(TravelDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<OperationResult<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await _ctx.Users
                .AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);

            if(exists)
            {
                return new OperationResult<RegisterResponse>
                {
                    IsSuccess = false,
                    Error = "User with given username or email already exists."
                };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 
            var user = new Domain.Entities.User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync(cancellationToken);

            return new OperationResult<RegisterResponse>
            {
                IsSuccess = true,
                Data = new RegisterResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Name = user.Name
                }
            };
        }
    }
}
