using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class BusinessProfileDto:UserDto
    {
        public BusinessType? BusinessType { get; set; }
        public string? BusinessAddress { get; set; } = null!;
        public string? BusinessWebsite { get; set; } = null!;
        public string? BusinessPhone { get; set; } = null!;
    }
}
