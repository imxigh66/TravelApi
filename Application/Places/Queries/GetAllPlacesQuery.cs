using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Queries
{
    public class GetAllPlacesQuery:IRequest<OperationResult<List<PlaceDto>>>
    {
    }
}
