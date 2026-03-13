using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Commands
{
    public class UnsavePlaceCommand : IRequest<OperationResult<bool>>
    {
        public int UserId { get; set; }
        public int PlaceId { get; set; }
    }
}
