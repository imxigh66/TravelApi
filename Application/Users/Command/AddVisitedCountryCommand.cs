using Application.Common.Models;
using Application.DTO.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Command
{
    public class AddVisitedCountryCommand : IRequest<OperationResult<VisitedCountryDto>>
    {
        public int UserId { get; set; }
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateOnly? VisitedAt { get; set; }
        public string? Note { get; set; }
    }
}
