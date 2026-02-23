using Application.Common.Models;
using Application.DTO.CategoryTags;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.Commands
{
    public class CreateCategoryTagCommand:IRequest<OperationResult<CategoryTagDto>>
    {
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }
}
