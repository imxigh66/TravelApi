using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.Queries
{
    public class IsSavedQuery:IRequest<bool>
    {
        public int UserId { get; set; }
        public int PlaceId { get; set; }
    }
}
