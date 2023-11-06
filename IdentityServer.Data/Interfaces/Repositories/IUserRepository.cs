using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Dto;
using IdentityServer.Data.Models;

namespace IdentityServer.Data.Interfaces.Repositories; 

public interface IUserRepository {

    
    
    Task<Result<JwtModel>> LoginUserAsync(string email, string password);
    Task<Result<JwtModel>> RegisterUserAsync(RegisterModel registerModel);

    Task<Result> DeleteUserAsync(string userId);
}