using Application.DTO.CategoryTags;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.Queries
{
    public class GetAllCategoryTagsQuery : IRequest<List<CategoryTagDto>>
    {
    }
}
