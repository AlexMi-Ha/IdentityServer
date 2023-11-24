using FluentValidation;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Models;
using IdentityServer.Data.Services;
using Microsoft.AspNetCore.Identity;
using ValidationException = IdentityServer.Core.Exceptions.ValidationException;

namespace IdentityServer.Data.Repositories; 

internal class UserRepository : IUserRepository {
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenFactory _tokenFactory;
    private readonly IValidator<LoginModel> _loginValidator;
    private readonly IValidator<RegisterModel> _registerValidator;
    public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        TokenFactory tokenFactory, IValidator<LoginModel> loginValidator, IValidator<RegisterModel> registerValidator) {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenFactory = tokenFactory;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }


    public async Task<Result<JwtModel>> LoginUserAsync(LoginModel model) {
        
        var validationRes = await _loginValidator.ValidateAsync(model);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null) {
            return new AuthFailureException("Invalid Email or Password!");
        }
        var attempt = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        if (attempt.IsLockedOut) {
            return new AuthFailureException("User is locked out!");
        }
        if (!attempt.Succeeded) {
            return new AuthFailureException("Invalid Email or Password!");
        }
        // Success
        var roles = await _userManager.GetRolesAsync(user);
        var claims = _tokenFactory.GetUserAuthClaims(user, roles);
        return _tokenFactory.GenerateJwtSecurityToken(claims);
    }

    public async Task<Result<JwtModel>> RegisterUserAsync(RegisterModel registerModel) {
        
        var validationRes = await _registerValidator.ValidateAsync(registerModel);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        var user = await _userManager.FindByEmailAsync(registerModel.Email);
        if (user is not null) {
            return new UserOperationException("User already exists!");
        }

        user = CreateBaseUserModel(registerModel.Email, registerModel.Name);

        var result = await _userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded) {
            return new UserOperationException("Failed to create account");
        }

        return await LoginUserAsync(new LoginModel{Email = registerModel.Email,Password = registerModel.Password});
    }

    public async Task<Result> DeleteUserAsync(string userId) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserNotFoundException("User does not exist");
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? true : new UserOperationException("Failed deleting the user");
    }

    public async Task<Result<UserModel>> GetUserAsync(string userId) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserNotFoundException();
        }
        return user.MapToDto();
    }

    private ApplicationUser CreateBaseUserModel(string email, string? name) {
        return new ApplicationUser() {
            Email = email,
            UserName = name,
            EmailConfirmed = false,
            LockoutEnabled = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false
        };
    }
}