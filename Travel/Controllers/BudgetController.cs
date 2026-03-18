using Application.DTO.Trips.Budget;
using Application.Trips.Commands;
using Application.Trips.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TravelApi.Controllers
{
    [ApiController]
    [Route("api/trips/{tripId:int}/budget")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BudgetController(IMediator mediator) => _mediator = mediator;

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

      
        [HttpGet]
        public async Task<IActionResult> Get(int tripId)
        {
            var result = await _mediator.Send(new GetBudgetQuery { TripId = tripId, UserId = UserId });
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Data);
        }

    
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(int tripId, [FromBody] CreateBudgetRequest request)
        {
            var result = await _mediator.Send(new CreateBudgetCommand
            {
                TripId = tripId,
                UserId = UserId,
                Currency = request.Currency,
                TotalLimit = request.TotalLimit,
            });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }

     
        [HttpPost("expenses")]
        public async Task<IActionResult> AddExpense(int tripId, [FromBody] CreateExpenseRequest request)
        {
            var result = await _mediator.Send(new AddExpenseCommand
            {
                TripId = tripId,
                UserId = UserId,
                Category = request.Category,
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date,
            });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Data);
        }

    
        [HttpDelete("expenses/{expenseId:int}")]
        public async Task<IActionResult> DeleteExpense(int tripId, int expenseId)
        {
            var result = await _mediator.Send(new DeleteExpenseCommand
            {
                ExpenseId = expenseId,
                TripId = tripId,
                UserId = UserId,
            });
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok();
        }
    }
}
