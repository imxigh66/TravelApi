using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Command;
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
    public class UpdatePersonalProfileCommandHandler : IRequestHandler<UpdatePersonalProfileCommand, OperationResult<PersonalProfileDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdatePersonalProfileCommandHandler> _logger;
        public UpdatePersonalProfileCommandHandler(IApplicationDbContext context, ILogger<UpdatePersonalProfileCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<OperationResult<PersonalProfileDto>> Handle(UpdatePersonalProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return OperationResult<PersonalProfileDto>.Failure("User not found.");

            }

            if (user.AccountType != AccountType.Personal)
                return OperationResult<PersonalProfileDto>.Failure("Only personal accounts can update this profile");

            user.Name = request.Name ?? user.Name;
            user.Username = request.Username ?? user.Username;
            user.Country = request.Country ?? user.Country;
            user.City = request.City ?? user.City;
            user.Bio = request.Bio ?? user.Bio;
            user.ProfilePicture = request.ProfilePicture ?? user.ProfilePicture;
            user.TravelInterest = request.TravelInterest;
            user.TravelStyle = request.TravelStyle;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var dto = new PersonalProfileDto
            {
                Name = user.Name,
                Username = user.Username,
                Country = user.Country,
                City = user.City,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                TravelInterest = user.TravelInterest,
                TravelStyle = user.TravelStyle
            };

            return OperationResult<PersonalProfileDto>.Success(dto);
        }
    }
}
