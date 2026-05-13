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
    public class StartConversationCommand : IRequest<OperationResult<ConversationDto>>
    {
        public int CurrentUserId { get; set; }
        public int OtherUserId { get; set; }
    }
}
