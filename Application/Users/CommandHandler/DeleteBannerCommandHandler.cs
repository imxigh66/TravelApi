using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CommandHandler
{
    public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, OperationResult<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public DeleteBannerCommandHandler(IApplicationDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<OperationResult<bool>> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
            if (user == null)
                return OperationResult<bool>.Failure("User not found.");

            if (!string.IsNullOrEmpty(user.BannerImage))
                await _fileStorageService.DeleteFileAsync(user.BannerImage, cancellationToken);

            user.BannerImage = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<bool>.Success(true);
        }
    }
}
