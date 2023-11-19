using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;

namespace IdentityServer.Core.Interfaces.Repositories; 

public interface IRoleRepository {

    Task<Result<IEnumerable<RoleModel>>> GetAllRolesAsync();
    Task<Result<IEnumerable<UserModel>>> GetAllUsersInRoleAsync(string roleName);

    Task<Result<RoleModel>> AddNewRoleAsync(RoleModel model);
    Task<Result> DeleteRoleAsync(string name);

    Task<Result> AddUserToRoleAsync(string userId, string roleName);

}