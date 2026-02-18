using Application.CategoryTags.Queries;
using Application.Common.Interfaces;
using Application.DTO.CategoryTags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.QueryHandler
{
    public class GetAllCategoryTagsQueryHandler : IRequestHandler<Queries.GetAllCategoryTagsQuery, List<DTO.CategoryTags.CategoryTagDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllCategoryTagsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryTagDto>> Handle(GetAllCategoryTagsQuery request, CancellationToken cancellationToken)
        {
            return await _context.CategoryTags
                .OrderBy(c=>c.Name)
                .Select(ct => new CategoryTagDto
                {
                    CategoryTagId = ct.CategoryTagId,
                    Name = ct.Name,
                    Icon = ct.Icon,
                    CreatedAt = ct.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
