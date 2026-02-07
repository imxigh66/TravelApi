using Application.Common.Models;
using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueriesCommand
{
    public class GetUserByIdQuery:IRequest<OperationResult<UserResponse>>
    {
        public int UserId { get; set; }
    }
}
