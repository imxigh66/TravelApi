using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Users;
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
    public class AddVisitedCountryCommandHandler
       : IRequestHandler<AddVisitedCountryCommand, OperationResult<VisitedCountryDto>>
    {
        private readonly IApplicationDbContext _context;
        public AddVisitedCountryCommandHandler(IApplicationDbContext context)
            => _context = context;

        public async Task<OperationResult<VisitedCountryDto>> Handle(
            AddVisitedCountryCommand request, CancellationToken cancellationToken)
        {
            var cc = request.CountryCode.ToUpper().Trim();
            if (cc.Length != 2)
                return OperationResult<VisitedCountryDto>.Failure("Invalid country code.");


            var exists = await _context.UserVisitedCountries
                .AnyAsync(c => c.UserId == request.UserId && c.CountryCode == cc, cancellationToken);

            if (exists)
                return OperationResult<VisitedCountryDto>.Failure("Country already added.");

            var entity = new UserVisitedCountry
            {
                UserId = request.UserId,
                CountryCode = cc,
                City = request.City,
                VisitedAt = request.VisitedAt,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
            };

            _context.UserVisitedCountries.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<VisitedCountryDto>.Success(new VisitedCountryDto
            {
                Id = entity.Id,
                CountryCode = entity.CountryCode,
                City = entity.City,
                VisitedAt = entity.VisitedAt,
                Note = entity.Note,
                Source = "manual",
            });
        }
    }
}
