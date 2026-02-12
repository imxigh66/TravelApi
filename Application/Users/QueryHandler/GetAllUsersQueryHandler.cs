using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueryHandler
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper; 
        public GetAllUsersQueryHandler(IApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var usersQuery = _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            
            return await PaginatedList<UserDto>.CreateAsync(
                usersQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

        }
    }
}
