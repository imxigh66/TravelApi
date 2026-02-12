using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Command
{
    public class UpdateProfilePictureCommandValidator : AbstractValidator<UpdateProfilePictureCommand>
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public UpdateProfilePictureCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID is required");

            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image file is required");

            RuleFor(x => x.Image.Length)
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024} MB")
                .When(x => x.Image != null);

            RuleFor(x => x.Image.ContentType)
                .Must(contentType => AllowedContentTypes.Contains(contentType?.ToLower()))
                .WithMessage($"Invalid file type. Allowed types: {string.Join(", ", AllowedContentTypes)}")
                .When(x => x.Image != null);

            RuleFor(x => x.Image.FileName)
                .Must(fileName =>
                {
                    if (string.IsNullOrEmpty(fileName)) return false;
                    var extension = Path.GetExtension(fileName).ToLower();
                    return AllowedExtensions.Contains(extension);
                })
                .WithMessage($"Invalid file extension. Allowed extensions: {string.Join(", ", AllowedExtensions)}")
                .When(x => x.Image != null);
        }
    }
}
