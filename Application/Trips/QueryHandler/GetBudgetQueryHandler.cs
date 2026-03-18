using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Trips.Budget;
using Application.Trips.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.QueryHandler
{
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, OperationResult<BudgetDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetBudgetQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<OperationResult<BudgetDto>> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null)
                return OperationResult<BudgetDto>.Failure("Trip not found.");

            if (trip.OwnerId != request.UserId)
                return OperationResult<BudgetDto>.Failure("Access denied.");

            var budget = await _context.Budgets
                .AsNoTracking()
                .Include(b => b.Expenses)
                .FirstOrDefaultAsync(b => b.TripId == request.TripId, cancellationToken);

            // Если бюджет ещё не создан — возвращаем пустой
            if (budget == null)
                return OperationResult<BudgetDto>.Success(new BudgetDto
                {
                    TripId = request.TripId,
                    Currency = "EUR",
                    TotalLimit = 0,
                    TotalSpent = 0,
                    Expenses = new List<ExpenseDto>()
                });

            var dto = new BudgetDto
            {
                BudgetId = budget.BudgetId,
                TripId = budget.TripId,
                Currency = budget.Currency,
                TotalLimit = budget.TotalLimit,
                TotalSpent = budget.Expenses.Sum(e => e.Amount),
                Expenses = budget.Expenses
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.CreatedAt)
                    .Select(e => new ExpenseDto
                    {
                        ExpenseId = e.ExpenseId,
                        BudgetId = e.BudgetId,
                        Category = e.Category,
                        Amount = e.Amount,
                        Description = e.Description,
                        Date = e.Date,
                        CreatedAt = e.CreatedAt,
                    }).ToList()
            };

            return OperationResult<BudgetDto>.Success(dto);
        }
    }
}
