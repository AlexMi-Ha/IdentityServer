using System.Security.Cryptography;
using System.Text;
using IdentityServer.Data.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Data.Services; 

internal static class KeyManager {

    public static void EnsureEncryptionKeys(this IServiceCollection services, IConfiguration config) {
        if (File.Exists(config.GetJwtPrivateKeyPath()) && File.Exists(config.GetJwtPublicKeyPath())) {
            return;
        }
        var keyDir = Path.GetDirectoryName(config.GetJwtKeyPath());
        if (keyDir is null) {
            throw new Exception("Unknown key directory!");
        }

        if (!Directory.Exists(keyDir)) {
            Directory.CreateDirectory(keyDir);
        }

        using var rsa = RSA.Create();
        var privateKey = rsa.ExportRSAPrivateKeyPem();
        var publicKey = rsa.ExportRSAPublicKeyPem();

        using var privateFile = File.Create(config.GetJwtPrivateKeyPath());
        using var publicFile = File.Create(config.GetJwtPublicKeyPath());
        
        privateFile.Write(Encoding.UTF8.GetBytes(privateKey));
        publicFile.Write(Encoding.UTF8.GetBytes(publicKey));
    }
}