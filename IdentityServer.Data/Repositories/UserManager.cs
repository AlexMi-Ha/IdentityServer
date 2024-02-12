using System.Linq.Expressions;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Models;
using IdentityServer.Data.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data.Repositories; 

public class UserManager : IUserManager<ApplicationUser> {

    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<RoleModel> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    
    public UserManager(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, RoleManager<RoleModel> roleManager) {
        _userManager = userManager;
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    public Task<List<ApplicationUser>> GetAllAsync() {
        return _userManager.Users.ToListAsync();
    }

    public Task<bool> AnyUsersAsync() {
        return _userManager.Users.AnyAsync();
    }

    public async Task<Result> CreateAsync(ApplicationUser user, string password) {
        var res = await _userManager.CreateAsync(user, password);
        return res.Succeeded ? true : new UserOperationException();
    }

    public async Task<Result> DeleteAsync(ApplicationUser user) {
        var res = await _userManager.DeleteAsync(user);
        return res.Succeeded ? true : new UserOperationException();
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId) {
        return _userManager.FindByIdAsync(userId);
    }

    public Task<ApplicationUser?> FindByNameAsync(string userName) {
        return _userManager.FindByNameAsync(userName);
    }

    public Task<ApplicationUser?> FindByEmailAsync(string email) {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password) {
        return _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<Result> AddToRoleAsync(ApplicationUser user, string role) {
        var res = await _userManager.AddToRoleAsync(user, role);
        return res.Succeeded ? true : new UserOperationException();
    }

    public async Task<Result> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles) {
        var res = await _userManager.AddToRolesAsync(user, roles);
        return res.Succeeded ? true : new UserOperationException();
    }

    public Task<List<RoleModel>> GetRolesAsync(ApplicationUser user) {
        var query = from userRole in _dbContext.UserRoles
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where userRole.UserId.Equals(user.Id)
            select new RoleModel() {
                RoleDescription = role.RoleDescription,
                RoleId = role.Id,
                RoleName = role.Name
            };
        return query.ToListAsync();
    }

    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role) {
        return _userManager.GetUsersInRoleAsync(role);
    }

    public Task<bool> IsInRoleAsync(ApplicationUser user, string role) {
        return _userManager.IsInRoleAsync(user, role);
    }

    public Task<List<string>> GetAllRolesAsync() {
        return _roleManager.Roles.Select(e => e.RoleName).ToListAsync();
    }

    public async Task<Result> RemoveUserFromRoleAsync(ApplicationUser user, string roleName) {
        return (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
    } 
}