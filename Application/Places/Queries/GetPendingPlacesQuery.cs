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
    public class GetPendingPlacesQuery : IRequest<PaginatedList<PlaceDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
