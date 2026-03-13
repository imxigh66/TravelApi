using Application.Common.Models;
using Application.DTO.Places;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.Queries
{
    public class GetPlacesByCategoryTagQuery : IRequest<PaginatedList<PlaceDto>>
    {
        public int CategoryTagId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
