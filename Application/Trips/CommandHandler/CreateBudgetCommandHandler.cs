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
    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, OperationResult<BudgetDto>>
    {
        private readonly IApplicationDbContext _context;
        public CreateBudgetCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<OperationResult<BudgetDto>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == request.TripId, cancellationToken);

            if (trip == null) return OperationResult<BudgetDto>.Failure("Trip not found.");
            if (trip.OwnerId != request.UserId) return OperationResult<BudgetDto>.Failure("Access denied.");

          
            var existing = await _context.Budgets
                .Include(b => b.Expenses)
                .FirstOrDefaultAsync(b => b.TripId == request.TripId, cancellationToken);

            if (existing != null)
            {
                existing.TotalLimit = request.TotalLimit;
                existing.Currency = request.Currency;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<BudgetDto>.Success(MapToDto(existing));
            }

            var budget = new Budget
            {
                TripId = request.TripId,
                Currency = request.Currency,
                TotalLimit = request.TotalLimit,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<BudgetDto>.Success(MapToDto(budget));
        }

        private static BudgetDto MapToDto(Budget b) => new()
        {
            BudgetId = b.BudgetId,
            TripId = b.TripId,
            Currency = b.Currency,
            TotalLimit = b.TotalLimit,
            TotalSpent = b.Expenses?.Sum(e => e.Amount) ?? 0,
            Expenses = b.Expenses?
                .Select(e => new ExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    BudgetId = e.BudgetId,
                    Category = e.Category,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.Date,
                    CreatedAt = e.CreatedAt,
                }).ToList() ?? new()
        };
    }
}
