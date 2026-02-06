using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class PersonalProfileDto : UserDto
    {
        public TravelInterest? TravelInterest { get; set; }
        public TravelStyle? TravelStyle { get; set; }
    }
}
