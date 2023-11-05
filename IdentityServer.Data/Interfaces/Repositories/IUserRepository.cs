using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Dto;

namespace IdentityServer.Data.Interfaces.Repositories; 

public interface IUserRepository {

    
    
    Task<Result<string>> LoginUserAsync(string email, string password);
    Task<Result<string>> RegisterUserAsync(RegisterModel registerModel);

    Task<Result> DeleteUserAsync(string userId);
}