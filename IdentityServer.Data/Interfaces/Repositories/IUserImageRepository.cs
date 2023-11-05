using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Interfaces.Misc;

namespace IdentityServer.Data.Interfaces.Repositories; 

public interface IUserImageRepository {

    Result<string> GetImagePathForUser(string userId);

    Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image);

    Task<Result> DeleteImageForUserAsync(string userId);

}