using Application.Common.Models;
using Application.DTO.Files;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Command
{
    public class UpdateProfilePictureCommand : IRequest<OperationResult<FileUploadResultDto>>
    {
        public int UserId { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
