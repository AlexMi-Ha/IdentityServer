using System.Security.Claims;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace IdentityServer.Data.Services; 

internal class TokenFactory {

    private readonly IJwtHandler _jwtHandler;
    public TokenFactory(IJwtHandler jwtHandler) {
        _jwtHandler = jwtHandler;
    }

    public JwtModel GenerateJwtSecurityToken(List<Claim> claims) {
        return _jwtHandler.Create(claims);
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