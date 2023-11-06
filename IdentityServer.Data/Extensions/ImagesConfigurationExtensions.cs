using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data.Extensions; 

public static class ImagesConfigurationExtensions {

    public static string GetImagesPath(this IConfiguration config) {
        return config["ImagePath"]!;
    }
}