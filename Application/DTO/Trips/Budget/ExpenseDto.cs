using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Budget
{
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public int BudgetId { get; set; }
        public string Category { get; set; } = "Other";
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
