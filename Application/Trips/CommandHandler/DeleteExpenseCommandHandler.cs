using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Trips.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteExpenseCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<OperationResult<bool>> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .Include(e => e.Budget)
                .FirstOrDefaultAsync(e => e.ExpenseId == request.ExpenseId, cancellationToken);

            if (expense == null) return OperationResult<bool>.Failure("Expense not found.");
            if (expense.Budget.TripId != request.TripId) return OperationResult<bool>.Failure("Access denied.");

         
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);
            if (trip?.OwnerId != request.UserId) return OperationResult<bool>.Failure("Access denied.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
