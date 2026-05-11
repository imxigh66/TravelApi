using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Reviews.Commands
{
    public class DeleteReviewCommand : IRequest<OperationResult<bool>>
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
    }
}
