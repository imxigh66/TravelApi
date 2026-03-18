using Application.Common.Interfaces;
using Application.DTO.Users;
using Application.Users.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.QueryHandler
{
    public class GetVisitedCountriesQueryHandler
         : IRequestHandler<GetVisitedCountriesQuery, List<VisitedCountryDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetVisitedCountriesQueryHandler(IApplicationDbContext context)
            => _context = context;

        public async Task<List<VisitedCountryDto>> Handle(
            GetVisitedCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.UserVisitedCountries
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new VisitedCountryDto
                {
                    Id = c.Id,
                    CountryCode = c.CountryCode,
                    City = c.City,
                    VisitedAt = c.VisitedAt,
                    Note = c.Note,
                    Source = "manual",
                })
                .ToListAsync(cancellationToken);
        }
    }
}
