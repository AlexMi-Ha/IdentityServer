using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Dto;

namespace IdentityServer.Core.Interfaces.Services; 

public interface IUserOperationService {
    
    Task<bool> IsNameAvailableAsync(string name);
    Task<Result> ChangeNameAsync(ChangeNameModel model);
}