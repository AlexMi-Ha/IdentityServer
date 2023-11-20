using FluentValidation;
using IdentityServer.Core.Dto;

namespace IdentityServer.Validation.Dto; 

public class RegisterValidator: AbstractValidator<RegisterModel> {


    public RegisterValidator() {
        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Email must be a valid email address!");

        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty")
            .MaximumLength(36)
            .WithMessage("Name cannot be longer than 36 characters")
            .Matches(@"[a-zA-Z][a-zA-Z0-9\s\-]*")
            .WithMessage("Names can only include alphanumeric characters, spaces or dashes!");

        RuleFor(e => e.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$")
            .WithMessage(
                "Password must have...\n\tat least one uppercase character\n\tat least one lowercase character\n\tat least one digit\n\tat least one special character\n\tat least 6 characters long");
    }
}