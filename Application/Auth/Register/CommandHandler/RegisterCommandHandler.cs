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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<RegisterResponse>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IMapper _mapper;
        public RegisterCommandHandler(IApplicationDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }
        public async Task<OperationResult<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await _ctx.Users
                .AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);

            if(exists)
            {
                return OperationResult<RegisterResponse>.Failure("User with given username or email already exists.");
               
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 
            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync(cancellationToken);

            return  OperationResult<RegisterResponse>.Success(new RegisterResponse
            {
               
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Name = user.Name
                
            });
        }
    }
}
