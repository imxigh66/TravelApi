using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Budget
{
    public class CreateBudgetRequest
    {
        public string Currency { get; set; } = "EUR";
        public decimal TotalLimit { get; set; }
    }
}
