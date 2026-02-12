using Application.Auth.Login.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Auth;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<LoginCommandHandler> _logger;
        public LoginCommandHandler(IApplicationDbContext context,IPasswordHasher passwordHasher,IJwtTokenService jwtTokenService, ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }
        public async Task<OperationResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                return OperationResult<LoginResponse>.Failure("Invalid email or password.");
                
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning($"⚠️ Login attempt with unconfirmed email: {request.Email}");
                return OperationResult<LoginResponse>.Failure(
                    "Email not confirmed. Please check your email or request a new confirmation link.");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return OperationResult<LoginResponse>.Failure("Invalid credentials");

          

            _logger.LogInformation($"✅ User logged in: {user.Email}");

            var accessToken = _jwtTokenService.GenerateAccessToken(
            user.UserId,
            user.Email,
            user.Username);

            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var refreshToken = new Domain.Entities.RefreshToken
            {
                UserId=user.UserId,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);


            var response = new LoginResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30) // Срок access token
            };

            return OperationResult<LoginResponse>.Success(response);

        }
    }
}
