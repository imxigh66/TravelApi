using Application.Auth.Register.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Auth;
using AutoMapper;
using Domain.Entities;
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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<RegisterDto>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        public RegisterCommandHandler(IApplicationDbContext ctx, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _ctx = ctx;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<OperationResult<RegisterDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await _ctx.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
            {
                return OperationResult<RegisterDto>.Failure("Email is already in use.");
            }

            var usernameExists = await _ctx.Users
                .AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);

            if(usernameExists)
            {
                return OperationResult<RegisterDto>.Failure("Username already exists");
               
            }

            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync(cancellationToken);

            return  OperationResult<RegisterDto>.Success(new RegisterDto
            {
               
                    Username = user.Username,
                    Email = user.Email,
                    Name = user.Name,
                    Password=string.Empty

            });
        }
    }
}
