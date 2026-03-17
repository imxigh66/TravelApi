using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
using Application.Users.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enum;
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
            var query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(term) ||
                    u.Username.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term));
            }

       
            if (!string.IsNullOrWhiteSpace(request.AccountType) &&
                Enum.TryParse<AccountType>(request.AccountType, true, out var accountTypeEnum))
            {
                query = query.Where(u => u.AccountType == accountTypeEnum);
            }

            query = request.SortBy switch
            {
                "name" => query.OrderBy(u => u.Name),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            var projected = query.ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            return await PaginatedList<UserDto>.CreateAsync(
                projected,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        }
    }
}
