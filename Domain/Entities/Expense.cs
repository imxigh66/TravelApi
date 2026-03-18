using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int BudgetId { get; set; }
        public string Category { get; set; } = "Other"; // Hotel, Food, Transport, Activities, Shopping, Other
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CreatedAt { get; set; }

        // NAV
        public Budget Budget { get; set; } = null!;
    }
}
