using FluentValidation;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using ValidationException = IdentityServer.Core.Exceptions.ValidationException;

namespace IdentityServer.Data.Services; 

internal class UserOperationService : IUserOperationService {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<ChangeNameModel> _nameValidator;
    public UserOperationService(UserManager<ApplicationUser> userManager, IValidator<ChangeNameModel> nameValidator) {
        _userManager = userManager;
        _nameValidator = nameValidator;
    }

    public async Task<bool> IsNameAvailableAsync(string name) {
        var validationRes = await _nameValidator.ValidateAsync(new ChangeNameModel(null, name));
        if (!validationRes.IsValid) {
            return false;
        }
        var user = await _userManager.FindByNameAsync(name);
        return user is null;
    }

    public async Task<Result> ChangeNameAsync(ChangeNameModel model) {
        
        var validationRes = await _nameValidator.ValidateAsync(model);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null) {
            return new UserNotFoundException();
        }

        user.UserName = model.NewName;
        var res = await _userManager.UpdateAsync(user);
        return res.Succeeded ? true : new UserOperationException();
    }
}