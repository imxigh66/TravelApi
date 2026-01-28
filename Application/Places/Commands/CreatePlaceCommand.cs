using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Commands
{
    public class CreatePlaceCommand:IRequest<OperationResult<PlaceDto>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string CountryCode { get; set; } = null!; // CHAR(2)
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public string? PlaceType { get; set; }
    }
}
