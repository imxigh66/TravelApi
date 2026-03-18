using Application.Common.Models;
using Application.DTO.Trips.Budget;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Queries
{
    public class GetBudgetQuery : IRequest<OperationResult<BudgetDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
    }
}
