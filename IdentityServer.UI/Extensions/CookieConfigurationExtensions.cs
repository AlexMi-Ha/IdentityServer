namespace IdentityServer.UI.Extensions; 

internal static class CookieConfigurationExtensions {

    internal static string GetCookieDomain(this IConfiguration config) {
        return config.GetCookie("Domain");
    }
    
    private static string GetCookie(this IConfiguration config, string val) {
        return config[$"Cookie{val}"]!;
    }
}
