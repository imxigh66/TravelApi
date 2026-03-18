using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips.Budget;
using Application.Trips.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.CommandHandler
{
    public class AddExpenseCommandHandler : IRequestHandler<AddExpenseCommand, OperationResult<ExpenseDto>>
    {
        private readonly IApplicationDbContext _context;
        public AddExpenseCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<OperationResult<ExpenseDto>> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null) return OperationResult<ExpenseDto>.Failure("Trip not found.");
            if (trip.OwnerId != request.UserId) return OperationResult<ExpenseDto>.Failure("Access denied.");

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.TripId == request.TripId, cancellationToken);

           
            if (budget == null)
            {
                budget = new Budget
                {
                    TripId = request.TripId,
                    Currency = "EUR",
                    TotalLimit = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var expense = new Expense
            {
                BudgetId = budget.BudgetId,
                Category = request.Category,
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<ExpenseDto>.Success(new ExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                BudgetId = expense.BudgetId,
                Category = expense.Category,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                CreatedAt = expense.CreatedAt,
            });
        }
    }
}
