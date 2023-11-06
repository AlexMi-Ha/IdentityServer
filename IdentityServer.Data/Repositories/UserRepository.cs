using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Dto;
using IdentityServer.Data.Exceptions;
using IdentityServer.Data.Interfaces.Repositories;
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
        if (attempt is null) {
            return new AuthFailureException("Authentication failed!");
        }
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

    public Task<Result<JwtModel>> RegisterUserAsync(RegisterModel registerModel) {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteUserAsync(string userId) {
        throw new NotImplementedException();
    }
}