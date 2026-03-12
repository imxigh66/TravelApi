using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.QueriesCommand;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueryHandler
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, OperationResult<UserResponse>>
    {
        private readonly IApplicationDbContext _context;
        public GetUserByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return  OperationResult<UserResponse>.Failure("User not found.");
               
            }

            var followersCount = await _context.UserFollows
    .CountAsync(f => f.FollowingId == request.UserId);

            var followingCount = await _context.UserFollows
                .CountAsync(f => f.FollowerId == request.UserId);

            var userDto = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture,
                Country = user.Country,
                City = user.City,
                AccountType = user.AccountType,
                TravelInterest = user.TravelInterest,
                TravelStyle = user.TravelStyle,
                BusinessType = user.BusinessType,
                BusinessAddress = user.BusinessAddress,
                BusinessWebsite = user.BusinessWebsite,
                BusinessPhone = user.BusinessPhone,
                Bio = user.Bio,
                FollowersCount = followersCount,  
                FollowingCount = followingCount,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return  OperationResult<UserResponse>.Success(userDto);
        }
    }
}
