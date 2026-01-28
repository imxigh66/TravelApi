using Application.DTO.Posts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Queries
{
    public class GetAllPostsQuery:IRequest<OperationResult<List<PostDto>>>
    {
    }
}
