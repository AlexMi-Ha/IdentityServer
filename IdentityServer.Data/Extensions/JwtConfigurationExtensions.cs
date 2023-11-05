using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data.Extensions; 

public static class JwtConfigurationExtensions {

    public static string GetJwtPrivateKeyPath(this IConfiguration config) {
        return Path.Combine(GetJwtKeyPath(config), "priv.key");
    }

    public static string GetJwtPublicKeyPath(this IConfiguration config) {
        return Path.Combine(GetJwtKeyPath(config), "pub.key");
    }

    public static string GetJwtKeyPath(this IConfiguration config) {
        return config.GetJwt("KeyPath");
    }
    
    public static string GetJwtIssuer(this IConfiguration config) {
        return config.GetJwt("Issuer");
    }

    public static string GetJwtAudience(this IConfiguration config) {
        return config.GetJwt("Audience");
    }
    
    private static string GetJwt(this IConfiguration config, string val) {
        return config[$"JwtConfig:{val}"]!;
    }
}