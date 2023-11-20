using System.Xml;
using FluentValidation;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = IdentityServer.Core.Exceptions.ValidationException;

namespace IdentityServer.Data.Repositories; 

internal class RoleRepository(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IValidator<RoleModel> roleValidator)
    : IRoleRepository {
    public async Task<Result<IEnumerable<RoleModel>>> GetAllRolesAsync() {
        return await roleManager.Roles
            .Select(e => new RoleModel() {
                    RoleId = e.Id,
                    RoleName = e.Name!,
                    RoleDescription = e.RoleDescription,
                }
            ).ToListAsync();
    }

    public async Task<Result<IEnumerable<UserModel>>> GetAllUsersInRoleAsync(string roleName) {
        var users = await userManager.GetUsersInRoleAsync(roleName);

        return new Result<IEnumerable<UserModel>>(
            users.Select(e => new UserModel {
                UserId = e.Id,
                Email = e.Email,
                EmailConfirmed = e.EmailConfirmed,
                LockedOut = e.LockoutEnd >= DateTimeOffset.UtcNow,
                UserName = e.UserName,
                MFAEnabled = e.TwoFactorEnabled
        }));
    }

    public async Task<Result<RoleModel>> AddNewRoleAsync(RoleModel model) {
        var validationRes = await roleValidator.ValidateAsync(model);
        if (!validationRes.IsValid) {
            return new ValidationException(validationRes.Errors);
        }
        
        if (await roleManager.RoleExistsAsync(model.RoleName)) {
            return new UserOperationException("Role does already exist");
        }

        var role = new ApplicationRole(model.RoleName, model.RoleDescription);
        var result = await roleManager.CreateAsync(role);
        return result.Succeeded
            ? model
            : new UserOperationException("Failed creating the role");
    }

    public async Task<Result> DeleteRoleAsync(string name) {
        var role = await roleManager.FindByNameAsync(name);
        if (role is null) {
            return new UserOperationException("Role does already exist");
        }

        var result = await roleManager.DeleteAsync(role);
        
        return result.Succeeded ? true : new UserOperationException("Failed deleting the role");
    }

    public async Task<Result> AddUserToRoleAsync(string userId, string roleName) {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) {
            return new UserOperationException("User does not exist");
        }
        var result = await userManager.AddToRoleAsync(user, roleName);

        return result.Succeeded ? true : new UserOperationException("Failed adding the user to the role");
    }
}