using Application.Common.Models;
using Application.DTO.Messages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.Queries
{
     public class GetMessagesQuery : IRequest<PaginatedList<MessageDto>>
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 30;
    }
}
