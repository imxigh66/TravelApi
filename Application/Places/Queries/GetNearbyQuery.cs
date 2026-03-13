using Application.Common.Models;
using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Queries
{
    public class GetNearbyQuery:IRequest<OperationResult<List<PlaceDto>>>
    {
        public int PlaceId { get; set; }
    }
}
