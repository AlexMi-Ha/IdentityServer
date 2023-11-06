using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Interfaces.Misc;
using IdentityServer.Data.Interfaces.Repositories;
using IdentityServer.Data.Services;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data.Repositories; 

internal class UserImageRepository : IUserImageRepository {

    private readonly IConfiguration _config;
    
    public UserImageRepository(IConfiguration config) {
        _config = config;
    }
    
    public Result<string> GetImagePathForUser(string userId) {
        var imageDirectory = _config.GetImagesPath();
        var userIdHash = Crypt.ComputePathSafeString(userId);
        var imagePath = Path.Combine(imageDirectory, userIdHash + ".jpg");
        if (!File.Exists(imagePath)) {
            return Path.Combine(imageDirectory, "default.jpg");
        }
        return imagePath;
    }

    public Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image) {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteImageForUserAsync(string userId) {
        throw new NotImplementedException();
    }
}