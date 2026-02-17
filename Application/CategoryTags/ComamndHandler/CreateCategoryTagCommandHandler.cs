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
    public class CreateCategoryTagCommandHandler : IRequestHandler<CreateCategoryTagCommand, OperationResult<CategoryTagDto>>
    {
        private readonly IApplicationDbContext _context;
        public CreateCategoryTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<CategoryTagDto>> Handle(CreateCategoryTagCommand request, CancellationToken cancellationToken)
        {
            var exists= await _context.CategoryTags.AnyAsync(ct => ct.Name == request.Name, cancellationToken);

            if (exists)
            {
                return OperationResult<CategoryTagDto>.Failure("A category tag with the same name already exists.");
            }

            var categoryTag = new Domain.Entities.CategoryTag
            {
                Name = request.Name,
                Icon = request.Icon,
                CreatedAt = DateTime.UtcNow
            };

            _context.CategoryTags.Add(categoryTag);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<CategoryTagDto>.Success(new CategoryTagDto
            {
                CategoryTagId = categoryTag.CategoryTagId,
                Name = categoryTag.Name,
                Icon = categoryTag.Icon,
                CreatedAt = categoryTag.CreatedAt
            });
        }
    }
}
