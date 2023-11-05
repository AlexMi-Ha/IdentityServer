using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Dto;

namespace IdentityServer.Data.Interfaces.Repositories; 

public interface IRoleRepository {

    Task<Result<IEnumerable<RoleModel>>> GetAllRolesAsync();
    Task<Result<IEnumerable<UserModel>>> GetAllUsersInRoleAsync(string roleName);

    Task<Result<RoleModel>> AddNewRoleAsync(string name);
    Task<Result> DeleteRoleAsync(string name);

    Task<Result> AddUserToRoleAsync(string userId, string roleName);

}