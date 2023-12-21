using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Interfaces.Misc;

namespace IdentityServer.Core.Interfaces.Repositories; 

public interface IUserImageRepository {

    string GetImagePathForUser(string userId, string webRootBasePath);

    Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image,string webRootBasePath);

    Result DeleteImageForUser(string userId, string webRootBasePath);

}