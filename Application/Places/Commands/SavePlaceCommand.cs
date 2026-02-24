using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Commands
{
    public class SavePlaceCommand:IRequest<OperationResult<bool>>
    {
        public int UserId { get; set; }

        public int PlaceId { get; set; }
    }
}
