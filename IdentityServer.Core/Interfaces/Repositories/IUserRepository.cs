using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;

namespace IdentityServer.Core.Interfaces.Repositories; 

public interface IUserRepository {

    
    
    Task<Result<JwtModel>> LoginUserAsync(string email, string password);
    Task<Result<JwtModel>> RegisterUserAsync(RegisterModel registerModel);

    Task<Result> DeleteUserAsync(string userId);
}