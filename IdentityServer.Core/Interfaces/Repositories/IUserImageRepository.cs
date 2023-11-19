using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Interfaces.Misc;

namespace IdentityServer.Core.Interfaces.Repositories; 

public interface IUserImageRepository {

    string GetImagePathForUser(string userId);

    Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image);

    Result DeleteImageForUser(string userId);

}