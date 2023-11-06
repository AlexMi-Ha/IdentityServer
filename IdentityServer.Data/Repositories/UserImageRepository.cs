using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Exceptions;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Interfaces.Misc;
using IdentityServer.Data.Interfaces.Repositories;
using IdentityServer.Data.Services;
using Microsoft.Extensions.Configuration;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace IdentityServer.Data.Repositories;

internal class UserImageRepository : IUserImageRepository {

    private readonly IConfiguration _config;

    public UserImageRepository(IConfiguration config) {
        _config = config;
    }

    public string GetImagePathForUser(string userId) {
        return GetImagePathForUser(userId, true);
    }

    private string GetImagePathForUser(string userId, bool checkForExistence) {
        var imageDirectory = _config.GetImagesPath();
        var userIdHash = Crypt.ComputePathSafeString(userId);
        var imagePath = Path.Combine(imageDirectory, userIdHash + ".jpg");
        if (checkForExistence && !File.Exists(imagePath)) {
            return Path.Combine(imageDirectory, "default.jpg");
        }
        return imagePath;
    }

    public async Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image) {
        // Is the image bigger than 2MiB
        if (image.Length > 2_097_152) {
            return new ImageOperationException("File must be smaller than 2MB!");
        }

        if (!image.IsImage()) {
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
                var path = GetImagePathForUser(userId, false);
                await imageModel.SaveAsJpegAsync(path);
                return path;
            }
            catch (Exception) {
                return new ImageOperationException("Error while saving the image!");
            }
        }
        catch (Exception) {
            return new ImageOperationException("Error while processing the image!");
        }

    }

    public Task<Result> DeleteImageForUserAsync(string userId) {
        throw new NotImplementedException();
    }
}