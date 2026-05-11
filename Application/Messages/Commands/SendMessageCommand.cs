using Application.Common.Models;
using Application.DTO.Messages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.Commands
{
    public class SendMessageCommand : IRequest<OperationResult<MessageDto>>
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
    }
}
