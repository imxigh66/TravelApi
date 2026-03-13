using Application.CategoryTags.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.ComamndHandler
{
    public class DeleteCategoryTagCommandHandler : IRequestHandler<DeleteCategoryTagCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCategoryTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<bool>> Handle(DeleteCategoryTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _context.CategoryTags
                .FindAsync(new object[] { request.CategoryTagId }, cancellationToken);

            if (tag == null)
                return OperationResult<bool>.Failure("Category tag not found");

            _context.CategoryTags.Remove(tag);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
