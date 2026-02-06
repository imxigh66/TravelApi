using Application.Auth.Register.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Auth;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Register.CommandHandler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<RegisterDto>>
    {
        private readonly IApplicationDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterCommandHandler> _logger;
        public RegisterCommandHandler(IApplicationDbContext ctx, IMapper mapper, IPasswordHasher passwordHasher, IEmailService emailService, ILogger<RegisterCommandHandler> logger)
        {
            _ctx = ctx;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _logger = logger;
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
            user.EmailConfirmed = false;
            user.EmailConfirmationToken = GenerateSecureToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync(cancellationToken);

            try
            {
                var confirmationLink = $"{request.BaseUrl}/api/auth/confirm-email?token={user.EmailConfirmationToken}";

                await _emailService.SendEmailConfirmationAsync(
                    user.Email,
                    user.Name,
                    confirmationLink);

                _logger.LogInformation($"✅ User registered: {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send confirmation email to {user.Email}");

            }

            return  OperationResult<RegisterDto>.Success(new RegisterDto
            {
               
                    Username = user.Username,
                    Email = user.Email,
                    Name = user.Name,
                    Password=string.Empty
                    
            });


        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
