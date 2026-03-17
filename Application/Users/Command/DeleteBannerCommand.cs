using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Command
{
    public class DeleteBannerCommand:IRequest<OperationResult<bool>>
    {
        public int UserId { get; set; }
    }
}
