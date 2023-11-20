using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Models;
using IdentityServer.Data.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Data.Repositories; 

internal class UserRepository : IUserRepository {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly TokenFactory _tokenFactory;
    
    public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenFactory tokenFactory) {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenFactory = tokenFactory;
    }

    public async Task<Result<JwtModel>> LoginUserAsync(LoginModel model) {
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

    public async Task<bool> IsNameAvailableAsync(string name) {
        var user = await _userManager.FindByNameAsync(name);
        return user is null;
    }

    public async Task<Result> ChangeNameAsync(string userId, string newName) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserNotFoundException();
        }

        user.UserName = newName;
        var res = await _userManager.UpdateAsync(user);
        return res.Succeeded ? true : new UserOperationException();
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