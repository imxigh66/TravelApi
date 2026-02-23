using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.Commands
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MaximumLength(5000).WithMessage("Content must not exceed 5000 characters");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Images)
                .Must(HaveAtMost10Images).WithMessage("Maximum 10 images allowed")
                .When(x => x.Images != null && x.Images.Any());

            RuleForEach(x => x.Images)
                .Must(BeAValidImage).WithMessage("Only image files are allowed (jpg, jpeg, png, gif, webp)")
                .Must(BeUnder5MB).WithMessage("Each image must be less than 5MB")
                .When(x => x.Images != null && x.Images.Any());

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID is required");
        }


        private bool HaveAtMost10Images(System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile>? images)
        {
            return images == null || images.Count <= 10;
        }

        private bool BeAValidImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        private bool BeUnder5MB(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;

            return file.Length <= 5 * 1024 * 1024; // 5MB
        }
    }
}
