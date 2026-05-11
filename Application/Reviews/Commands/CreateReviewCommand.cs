using Application.Common.Models;
using Application.DTO.Reviews;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.Commands
{
    public class CreateReviewCommand : IRequest<OperationResult<ReviewDto>>
    {
        public int PlaceId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
