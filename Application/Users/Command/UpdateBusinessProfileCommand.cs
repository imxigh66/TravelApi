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
    public class UpdateBusinessProfileCommand:IRequest<OperationResult<BusinessProfileDto>>
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public BusinessType? BusinessType { get; set; }
        public string? BusinessAddress { get; set; } = null!;
        public string? BusinessWebsite { get; set; } = null!;
        public string? BusinessPhone { get; set; } = null!;
    }
}
