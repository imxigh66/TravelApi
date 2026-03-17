using Application.Common.Models;
using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class GetAllUsersQuery:IRequest<PaginatedList<UserDto>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Search { get; set; }      // поиск по имени / username / email
        public string? AccountType { get; set; } 
        public string? SortBy { get; set; }
    }
}
