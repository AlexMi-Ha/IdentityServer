using FluentValidation;
using IdentityServer.Core.Dto;

namespace IdentityServer.Validation.Dto; 

public class RoleModelValidator : AbstractValidator<RoleModel> {

    public RoleModelValidator() {
        RuleFor(e => e.RoleId)
            .Must(e => e is null || string.IsNullOrEmpty(e) || Guid.TryParse(e, out _));
        
        RuleFor(e => e.RoleName)
            .NotEmpty()
            .WithMessage("Name cannot be empty")
            .MaximumLength(36)
            .WithMessage("Name cannot be longer than 36 characters")
            .Matches(@"[a-zA-Z][a-zA-Z0-9\-_]*")
            .WithMessage("Names can only include alphanumeric characters, dashes or underscores!");
        
        RuleFor(e => e.RoleDescription)
            .NotEmpty()
            .WithMessage("Description cannot be empty")
            .MaximumLength(255)
            .WithMessage("Description cannot be longer than 255 characters")
            .Matches(@"[a-zA-Z][a-zA-Z0-9\s\-_]*")
            .WithMessage("Descriptions can only include alphanumeric characters, spaces, dashes or underscores!");
    }
}