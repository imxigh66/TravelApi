using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Trips.Commands
{
    public class DeleteTripNoteCommand : IRequest<OperationResult<bool>>
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
    }
}
