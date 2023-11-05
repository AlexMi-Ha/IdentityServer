using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data.Extensions; 

public static class JwtConfigurationExtensions {

    public static string GetJwtPrivateKey(this IConfiguration config) {
        return config.GetJwt("Key");
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