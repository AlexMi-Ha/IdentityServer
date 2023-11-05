using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace IdentityServer.Data.Services; 

internal class TokenFactory {

    private readonly IConfiguration _config;

    public TokenFactory(IConfiguration config) {
        _config = config;
    }

    public async Task<string> GenerateJwtSecurityTokenAsync(List<Claim> claims) {
        claims.Add(new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Exp, EpochTime.GetIntDate(DateTime.Now.AddHours(12)).ToString()));

        using var rsa = RSA.Create();
        var keyXml = await File.ReadAllTextAsync(_config.GetJwtPrivateKeyPath());
        rsa.FromXmlString(keyXml);

        var signingKey = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        
        var jwt = new JwtSecurityToken(
            issuer: _config.GetJwtIssuer(),
            audience: _config.GetJwtAudience(),
            expires: DateTime.Now.AddHours(12),
            claims: claims,
            signingCredentials: signingKey
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    public List<Claim> GetUserAuthClaims(ApplicationUser user, IEnumerable<string> roles) {
        var authClaims = new List<Claim> {
            new (ClaimTypes.NameIdentifier, user.Id),
        };
        if (user.Email is not null) {
            authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            authClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
        }
        if (user.UserName is not null) {
            authClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        authClaims.AddRange(roles.Select(userRole => new Claim("roles", userRole)));

        return authClaims;
    }
}