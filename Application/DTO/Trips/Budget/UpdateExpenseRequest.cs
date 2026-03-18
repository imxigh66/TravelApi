using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Trips.Budget
{
    public class UpdateExpenseRequest
    {
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public DateOnly? Date { get; set; }
    }
}
