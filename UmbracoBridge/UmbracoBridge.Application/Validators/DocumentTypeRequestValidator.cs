using FluentValidation;
using UmbracoBridge.Application.DTOs;

namespace UmbracoBridge.Application.Validators;

public class DocumentTypeRequestValidator : AbstractValidator<DocumentTypeRequestDto>
{
    public DocumentTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Icon)
            .NotEmpty()
            .Must(icon => icon.StartsWith("icon-"))
            .WithMessage("Icon must start with 'icon-'.");
    }
}
