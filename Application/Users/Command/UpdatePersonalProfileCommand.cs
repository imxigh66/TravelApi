using Application.Common.Models;
using Application.DTO.Users;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Command
{
    public class UpdatePersonalProfileCommand:IRequest<OperationResult<PersonalProfileDto>>
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; } = string.Empty;  
        public TravelInterest TravelInterest { get; set; }
        public TravelStyle TravelStyle { get; set; }
    }
}
