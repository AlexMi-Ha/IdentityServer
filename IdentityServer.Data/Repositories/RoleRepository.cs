using System.Xml;
using FluentValidation;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = IdentityServer.Core.Exceptions.ValidationException;

namespace IdentityServer.Data.Repositories; 

internal class RoleRepository : IRoleRepository {
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<RoleModel> _roleValidator;
    public RoleRepository(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IValidator<RoleModel> roleValidator) {
        _roleManager = roleManager;
        _userManager = userManager;
        _roleValidator = roleValidator;
    }

    public async Task<Result<IEnumerable<RoleModel>>> GetAllRolesAsync() {
        return await _roleManager.Roles
            .Select(e => new RoleModel() {
                    RoleId = e.Id,
                    RoleName = e.Name!,
                    RoleDescription = e.RoleDescription,
                }
            ).ToListAsync();
    }

    public async Task<Result<IEnumerable<UserModel>>> GetAllUsersInRoleAsync(string roleName) {
        var users = await _userManager.GetUsersInRoleAsync(roleName);

        return new Result<IEnumerable<UserModel>>(
            users.Select(e => e.MapToDto()));
    }

    public async Task<Result<RoleModel>> AddNewRoleAsync(RoleModel model) {
        var validationRes = await _roleValidator.ValidateAsync(model);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        if (await _roleManager.RoleExistsAsync(model.RoleName)) {
            return new UserOperationException("Role does already exist");
        }

        var role = new ApplicationRole(model.RoleName, model.RoleDescription);
        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded
            ? model
            : new UserOperationException("Failed creating the role");
    }

    public async Task<Result> DeleteRoleAsync(string name) {
        var role = await _roleManager.FindByNameAsync(name);
        if (role is null) {
            return new UserOperationException("Role does already exist");
        }

        var result = await _roleManager.DeleteAsync(role);
        
        return result.Succeeded ? true : new UserOperationException("Failed deleting the role");
    }

    public async Task<Result> AddUserToRoleAsync(string userId, string roleName) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserOperationException("User does not exist");
        }
        var result = await _userManager.AddToRoleAsync(user, roleName);

        return result.Succeeded ? true : new UserOperationException("Failed adding the user to the role");
    }
}