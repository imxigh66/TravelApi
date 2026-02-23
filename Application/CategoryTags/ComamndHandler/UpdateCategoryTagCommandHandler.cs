using Application.CategoryTags.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.CategoryTags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.ComamndHandler
{
    public class UpdateCategoryTagCommandHandler : IRequestHandler<UpdateCategoryTagCommand, OperationResult<CategoryTagDto>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCategoryTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<CategoryTagDto>> Handle(UpdateCategoryTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _context.CategoryTags
                .FindAsync(new object[] { request.CategoryTagId }, cancellationToken);

            if (tag == null)
                return OperationResult<CategoryTagDto>.Failure("Category tag not found");

            var duplicate = await _context.CategoryTags
                .AnyAsync(c => c.Name == request.Name && c.CategoryTagId != request.CategoryTagId, cancellationToken);

            if (duplicate)
                return OperationResult<CategoryTagDto>.Failure("Category tag with this name already exists");

            tag.Name = request.Name;
            tag.Icon = request.Icon;

            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<CategoryTagDto>.Success(new CategoryTagDto
            {
                CategoryTagId = tag.CategoryTagId,
                Name = tag.Name,
                Icon = tag.Icon,
                CreatedAt = tag.CreatedAt
            });
        }
    }
}
