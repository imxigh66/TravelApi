using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CategoryTags.Commands
{
    public class AssignTagsToPlaceCommand : IRequest<OperationResult<bool>>
    {
        public int PlaceId { get; set; }
        public List<int> CategoryTagIds { get; set; } = new();
    }
}
