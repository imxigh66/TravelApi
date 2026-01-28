using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class GetAllUsersQuery:IRequest<OperationResult<List<UserDto>>>
    {
    }
}
