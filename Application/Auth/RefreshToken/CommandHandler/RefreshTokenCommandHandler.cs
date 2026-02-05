using Application.Auth.RefreshToken.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.RefreshToken.CommandHandler
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, OperationResult<LoginResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<OperationResult<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked, cancellationToken);

            if (refreshToken==null)
            {
                return OperationResult<LoginResponse>.Failure("Invalid refresh token");
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return OperationResult<LoginResponse>.Failure("Refresh token has expired");
            }

            var user=refreshToken.User;
            var newAccessToken = _jwtTokenService.GenerateAccessToken(
                user.UserId,
                user.Email,
                user.Username);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            refreshToken.IsRevoked = true;

            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new LoginResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30) // Срок access token
            };

            return OperationResult<LoginResponse>.Success(response);
        }
    }
}
