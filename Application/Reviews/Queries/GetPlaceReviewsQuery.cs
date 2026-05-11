using Application.Common.Models;
using Application.DTO.Reviews;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.Queries
{
    public class GetPlaceReviewsQuery : IRequest<PaginatedList<ReviewDto>>
    {
        public int PlaceId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
