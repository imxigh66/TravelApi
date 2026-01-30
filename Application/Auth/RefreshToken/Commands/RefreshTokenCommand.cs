using Application.Common.Models;
using Application.DTO.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.RefreshToken.Commands
{
    public class RefreshTokenCommand : IRequest<OperationResult<LoginResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
