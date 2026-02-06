using Application.Common.Models;
using Application.DTO.Auth;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Register.Commands
{
    public class RegisterCommand:IRequest<OperationResult<RegisterDto>>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AccountType AccountType { get; set; } = AccountType.Personal;
        public TravelInterest TravelInterest { get; set; }
        public TravelStyle TravelStyle { get; set; }

        

        public string? BaseUrl { get; set; }
    }
}
