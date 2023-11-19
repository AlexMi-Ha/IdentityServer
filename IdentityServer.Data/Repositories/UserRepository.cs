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

    public async Task<Result<JwtModel>> LoginUserAsync(string email, string password) {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) {
            return new AuthFailureException("Invalid Email or Password!");
        }
        var attempt = await _signInManager.CheckPasswordSignInAsync(user, password, true);
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

        return await LoginUserAsync(registerModel.Email, registerModel.Password);
    }

    public async Task<Result> DeleteUserAsync(string userId) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserOperationException("User does not exist");
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? true : new UserOperationException("Failed deleting the user");
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