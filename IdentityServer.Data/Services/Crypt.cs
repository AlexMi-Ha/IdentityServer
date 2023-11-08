using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Data.Services;

internal static class Crypt {

    public static string ComputePathSafeString(string plaintext) {
        var bytes = HashMd5(plaintext);
        return new Guid(bytes).ToString();
    }
    
    public static byte[] HashMd5(string plainText) {
        return MD5.HashData(Encoding.ASCII.GetBytes(plainText));
    }
}
