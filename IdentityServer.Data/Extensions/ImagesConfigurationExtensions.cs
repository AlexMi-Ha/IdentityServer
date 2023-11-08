using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data.Extensions; 

internal static class ImagesConfigurationExtensions {

    public static string GetImagesPath(this IConfiguration config) {
        return config["ImagePath"]!;
    }
}