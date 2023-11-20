using FluentValidation;
using IdentityServer.Core.Dto;

namespace IdentityServer.Validation.Dto; 

public class ChangeNameModelValidator : AbstractValidator<ChangeNameModel> {

    public ChangeNameModelValidator() {
        RuleFor(e => e.NewName)
            .NotEmpty()
            .WithMessage("Name cannot be empty")
            .MaximumLength(36)
            .WithMessage("Name cannot be longer than 36 characters")
            .Matches(@"[a-zA-Z][a-zA-Z0-9\s\-]*")
            .WithMessage("Names can only include alphanumeric characters, spaces or dashes!");
    }
}