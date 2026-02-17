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
    public class UpdateCategoryTagCommand : IRequest<OperationResult<CategoryTagDto>>
    {
        public int CategoryTagId { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }
}
