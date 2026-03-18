using Application.Common.Models;
using Application.DTO.Trips.Budget;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class AddExpenseCommand : IRequest<OperationResult<ExpenseDto>>
    {
        public int TripId { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; } = "Other";
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
    }
}
