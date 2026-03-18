using Application.Common.Interfaces;
using Application.Users.Command;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class SyncCountryFromTripCommandHandler
        : IRequestHandler<SyncCountryFromTripCommand>
    {
        private readonly IApplicationDbContext _context;
        public SyncCountryFromTripCommandHandler(IApplicationDbContext context)
            => _context = context;

        public async Task Handle(SyncCountryFromTripCommand request, CancellationToken cancellationToken)
        {
            var cc = request.CountryCode.ToUpper().Trim();
            if (cc.Length != 2) return;

            var exists = await _context.UserVisitedCountries
                .AnyAsync(c => c.UserId == request.UserId && c.CountryCode == cc, cancellationToken);

            if (!exists)
            {
                _context.UserVisitedCountries.Add(new UserVisitedCountry
                {
                    UserId = request.UserId,
                    CountryCode = cc,
                    City = request.City,
                    CreatedAt = DateTime.UtcNow,
                });
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
