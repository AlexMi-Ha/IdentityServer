using FluentValidation;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using ValidationException = IdentityServer.Core.Exceptions.ValidationException;

namespace IdentityServer.Data.Services; 

public class UserOperationService(UserManager<ApplicationUser> userManager, IValidator<ChangeNameModel> nameValidator) : IUserOperationService {
    
    public async Task<bool> IsNameAvailableAsync(string name) {
        var user = await userManager.FindByNameAsync(name);
        return user is null;
    }

    public async Task<Result> ChangeNameAsync(ChangeNameModel model) {
        
        var validationRes = await nameValidator.ValidateAsync(model);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        var user = await userManager.FindByIdAsync(model.UserId);
        if (user is null) {
            return new UserNotFoundException();
        }

        user.UserName = model.NewName;
        var res = await userManager.UpdateAsync(user);
        return res.Succeeded ? true : new UserOperationException();
    }
}