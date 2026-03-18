using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class RemoveVisitedCountryCommandHandler
         : IRequestHandler<RemoveVisitedCountryCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        public RemoveVisitedCountryCommandHandler(IApplicationDbContext context)
            => _context = context;

        public async Task<OperationResult<bool>> Handle(
            RemoveVisitedCountryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.UserVisitedCountries
                .FirstOrDefaultAsync(c => c.UserId == request.UserId
                                       && c.CountryCode == request.CountryCode.ToUpper(),
                                    cancellationToken);

            if (entity == null)
                return OperationResult<bool>.Failure("Country not found.");

            _context.UserVisitedCountries.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
