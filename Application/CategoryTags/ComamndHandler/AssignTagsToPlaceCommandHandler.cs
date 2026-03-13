using Application.CategoryTags.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.ComamndHandler
{
    public class AssignTagsToPlaceCommandHandler : IRequestHandler<AssignTagsToPlaceCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;

        public AssignTagsToPlaceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<bool>> Handle(AssignTagsToPlaceCommand request, CancellationToken cancellationToken)
        {
            var placeExists=await _context.Places.AnyAsync(p => p.PlaceId == request.PlaceId, cancellationToken);
            if(!placeExists)
            {
                return OperationResult<bool>.Failure("Place not found");
            }

            var exitingLinks = _context.CategoryTagLinks.Where(pct => pct.PlaceId == request.PlaceId).ToListAsync(cancellationToken);

            _context.CategoryTagLinks.RemoveRange(await exitingLinks);

            foreach (var tagId in request.CategoryTagIds)
            {
                var tagExists = await _context.CategoryTags.AnyAsync(ct => ct.CategoryTagId == tagId, cancellationToken);
                if (!tagExists)
                {
                    return OperationResult<bool>.Failure($"Category tag with ID {tagId} not found");
                }

                _context.CategoryTagLinks.Add(new Domain.Entities.CategoryTagLink
                {
                    PlaceId = request.PlaceId,
                    CategoryTagId = tagId
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<bool>.Success(true);
        }
    }
}
