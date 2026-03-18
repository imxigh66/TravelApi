using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int TripId { get; set; }
        public string Currency { get; set; } = "EUR";
        public decimal TotalLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // NAV
        public Trip Trip { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
