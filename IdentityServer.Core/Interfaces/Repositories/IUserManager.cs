using System.Linq.Expressions;
using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;

namespace IdentityServer.Core.Interfaces.Repositories;

public interface IUserManager<T> {
    Task<List<T>> GetAllAsync();

    Task<bool> AnyUsersAsync();
    
    Task<Result> CreateAsync(T user, string password);

    Task<Result> DeleteAsync(T user);

    Task<T?> FindByIdAsync(string userId);

    Task<T?> FindByNameAsync(string userName);

    Task<T?> FindByEmailAsync(string email);

    Task<bool> CheckPasswordAsync(T user, string password);

    Task<Result> AddToRoleAsync(T user, string role);

    Task<Result> AddToRolesAsync(T user, IEnumerable<string> roles);

    Task<List<RoleModel>> GetRolesAsync(T user);
    Task<IList<T>> GetUsersInRoleAsync(string role);

    Task<bool> IsInRoleAsync(T user, string role);

    Task<List<string>> GetAllRolesAsync();
    Task<List<RoleModel>> GetAllRolesModelsAsync();
    Task<Result> AddNewRoleAsync(string name, string description);
    
    Task<Result> RemoveUserFromRoleAsync(T user, string roleName);
}