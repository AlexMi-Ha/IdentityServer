using IdentityServer.Core.Abstractions;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Misc;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Core.Services;
using IdentityServer.Data.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Data.Repositories;

internal class UserImageRepository : IUserImageRepository {

    private readonly IConfiguration _config;
    private readonly ILogger<IUserImageRepository> _logger;

    public UserImageRepository(IConfiguration config, ILogger<IUserImageRepository> logger) {
        _config = config;
        _logger = logger;
    }

    public string GetImagePathForUser(string userId, string webRootBasePath) {
        return GetImagePathForUser(userId, true, webRootBasePath);
    }

    private string GetImagePathForUser(string userId, bool checkForExistence, string webRootBasePath) {
        var imageDirectory = _config.GetImagesPath();
        var userIdHash = Crypt.ComputePathSafeString(userId);
        var imagePath = Path.Combine(imageDirectory, userIdHash + ".jpg");
        if (checkForExistence && !File.Exists(webRootBasePath + imagePath)) {
            return Path.Combine(imageDirectory, "default.jpg");
        }
        return imagePath;
    }

    public async Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image, string webRootBasePath) {
        // Is the image bigger than 2MiB
        if (image.Length > 2_097_152) {
            return new ImageOperationException("File must be smaller than 2MB!");
        }

        if (!image.IsImage()) {
            if (_logger.IsEnabled(LogLevel.Warning)) {
                _logger.LogWarning("Found file that is not an image uploaded by {userId}", userId);
            }
            return new ImageOperationException("Uploaded File must be an image!");
        }

        try {
            await using var imageStream = image.OpenReadStream();
            using var imageModel = await Image.LoadAsync(imageStream);
            if (imageModel.Width != imageModel.Height) {
                return new ImageOperationException("Image must be a square!");
            }

            try {
                imageModel.Mutate(x => x
                    .Resize(300, 300)
                );
            }
            catch (Exception) {
                return new ImageOperationException("Failed to resize the image!");
            }

            try {
                var path = GetImagePathForUser(userId, false, "");
                await imageModel.SaveAsJpegAsync(webRootBasePath + path);
                return path;
            }
            catch (Exception ex) {
                if (_logger.IsEnabled(LogLevel.Warning)) {
                    _logger.LogWarning("Could not save an image\nProduced Exception:{ex}",ex);
                }
                return new ImageOperationException("Error while saving the image!");
            }
        }
        catch (Exception ex) {
            if (_logger.IsEnabled(LogLevel.Warning)) {
                _logger.LogWarning("Could not process an image\nProduced Exception:{ex}", ex);
            }
            return new ImageOperationException("Error while processing the image!");
        }

    }

    public Result DeleteImageForUser(string userId, string webRootBasePath) {
        var path = GetImagePathForUser(userId, false, webRootBasePath);
        if (File.Exists(path)) {
            try {
                File.Delete(path);
            }
            catch (Exception ex) {
                if (_logger.IsEnabled(LogLevel.Warning)) {
                    _logger.LogWarning("Could not delete an image\nProduced Exception:{ex}",ex);
                }

                return false;
            }
            
        }

        return true;
    }
}