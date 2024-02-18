using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;

namespace IdentityServer.Core.Interfaces.Repositories; 

public interface IUserRepository {

    
    
    Task<Result<JwtModel>> LoginUserAsync(LoginModel model);
    Task<Result<JwtModel>> RegisterUserAsync(RegisterModel registerModel);
    Task<Result> CreateUserAsync(RegisterModel registerModel);

    Task<Result> DeleteUserAsync(string userId);
    Task<Result<UserModel>> GetUserByIdAsync(string userId);
    Task<Result<UserModel>> GetUserByEmailAsync(string email);
    Task<List<UserModel>> GetUsersAsync();
    Task<bool> AnyUsersAsync();

    Task<List<RoleModel>> GetUserRolesAsync(string userId);
    Task<List<string>> GetAllRolesAsync();
    Task<List<RoleModel>> GetAllRoleModelsAsync();

    Task<Result> AddNewRoleAsync(string name, string description);
    
    Task<Result> RemoveUserFromRoleAsync(string userId, string roleName);

    Task<Result> AddUserToRoleAsync(string userId, string roleName);

}