using Application.Common.Interfaces;
using Application.Places.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class IsSavedQueryHandler : IRequestHandler<IsSavedQuery, bool>
    {
        private readonly IApplicationDbContext _context;
        public IsSavedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(IsSavedQuery request, CancellationToken cancellationToken)
        {
            var isSaved = await _context.SavedPlaces
        .AnyAsync(sp => sp.UserId == request.UserId && sp.PlaceId == request.PlaceId);

            return isSaved;
        }
    }
}
