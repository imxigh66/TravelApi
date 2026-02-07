using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Command;
using AutoMapper;
using Domain.Enum;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class UpdateBusinessProfileCommandHandler : IRequestHandler<UpdateBusinessProfileCommand, OperationResult<BusinessProfileDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBusinessProfileCommandHandler> _logger;
        public UpdateBusinessProfileCommandHandler(IApplicationDbContext context,IMapper mapper,ILogger<UpdateBusinessProfileCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<OperationResult<BusinessProfileDto>> Handle(
       UpdateBusinessProfileCommand request,
       CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning($"User {request.UserId} not found");
                    return OperationResult<BusinessProfileDto>.Failure("User not found.");
                }

                if (user.AccountType != AccountType.Business)
                {
                    _logger.LogWarning($"User {request.UserId} is not a business account");
                    return OperationResult<BusinessProfileDto>.Failure(
                        "Only business accounts can update this profile");
                }

                // Обновления
                if (!string.IsNullOrWhiteSpace(request.Name))
                    user.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Username))
                    user.Username = request.Username;

                if (request.Country != null)
                    user.Country = string.IsNullOrWhiteSpace(request.Country) ? null : request.Country;

                if (request.City != null)
                    user.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City;

                if (request.Bio != null)
                    user.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio;

                if (request.ProfilePicture != null)
                    user.ProfilePicture = string.IsNullOrWhiteSpace(request.ProfilePicture) ? null : request.ProfilePicture;

                if (request.BusinessType.HasValue)
                    user.BusinessType = request.BusinessType;

                if (request.BusinessAddress != null)
                    user.BusinessAddress = string.IsNullOrWhiteSpace(request.BusinessAddress) ? null : request.BusinessAddress;

                if (request.BusinessWebsite != null)
                    user.BusinessWebsite = string.IsNullOrWhiteSpace(request.BusinessWebsite) ? null : request.BusinessWebsite;

                if (request.BusinessPhone != null)
                    user.BusinessPhone = string.IsNullOrWhiteSpace(request.BusinessPhone) ? null : request.BusinessPhone;

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Business profile updated for user {request.UserId}");

                // ✅ Мапим в UserResponse, который содержит ВСЕ поля включая AccountType!
                var dto = _mapper.Map<BusinessProfileDto>(user);

                return OperationResult<BusinessProfileDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating business profile for user {request.UserId}");
                return OperationResult<BusinessProfileDto>.Failure(
                    $"An error occurred while updating profile: {ex.Message}");
            }
        }
    }
}
