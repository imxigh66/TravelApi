using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Command;
using AutoMapper;
using Domain.Entities;
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
    public class UpdatePersonalProfileCommandHandler
    : IRequestHandler<UpdatePersonalProfileCommand, OperationResult<PersonalProfileDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePersonalProfileCommandHandler> _logger;

        public UpdatePersonalProfileCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ILogger<UpdatePersonalProfileCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationResult<PersonalProfileDto>> Handle(
            UpdatePersonalProfileCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Используй Users.FindAsync - он автоматически трекается
                var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning($"User {request.UserId} not found");
                    return OperationResult<PersonalProfileDto>.Failure("User not found.");
                }

                if (user.AccountType != AccountType.Personal)
                {
                    _logger.LogWarning($"User {request.UserId} is not a personal account");
                    return OperationResult<PersonalProfileDto>.Failure(
                        "Only personal accounts can update this profile");
                }

                // ✅ Обновляем только переданные поля
                if (!string.IsNullOrWhiteSpace(request.Name))
                    user.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Username))
                    user.Username = request.Username;

                if (request.Country != null) // позволяет установить null
                    user.Country = string.IsNullOrWhiteSpace(request.Country) ? null : request.Country;

                if (request.City != null)
                    user.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City;

                if (request.Bio != null)
                    user.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio;

                if (request.ProfilePicture != null)
                    user.ProfilePicture = string.IsNullOrWhiteSpace(request.ProfilePicture) ? null : request.ProfilePicture;

               

                user.UpdatedAt = DateTime.UtcNow;

                // ✅ Сохраняем изменения
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"User {request.UserId} profile updated successfully");

                // ✅ Возвращаем обновленные данные
                var dto = _mapper.Map<PersonalProfileDto>(user);

                return OperationResult<PersonalProfileDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile for user {request.UserId}");
                return OperationResult<PersonalProfileDto>.Failure(
                    $"An error occurred while updating profile: {ex.Message}");
            }
        }
    }
}
