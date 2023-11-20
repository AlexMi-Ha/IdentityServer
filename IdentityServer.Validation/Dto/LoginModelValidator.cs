using FluentValidation;
using IdentityServer.Core.Dto;

namespace IdentityServer.Validation.Dto; 

public class LoginModelValidator : AbstractValidator<LoginModel> {

    public LoginModelValidator() {
        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(e => e.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$");
    }
}