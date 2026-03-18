using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Budget
{
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public int TripId { get; set; }
        public string Currency { get; set; } = "EUR";
        public decimal TotalLimit { get; set; }
        public decimal TotalSpent { get; set; }           // сумма всех расходов
        public decimal Remaining => TotalLimit - TotalSpent;
        public List<ExpenseDto> Expenses { get; set; } = new();
    }
}
