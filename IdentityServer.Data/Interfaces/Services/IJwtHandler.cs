using System.Security.Claims;
using IdentityServer.Data.Dto;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Data.Interfaces.Services; 

public interface IJwtHandler {
    
    JwtModel Create(List<Claim> userClaims);
    TokenValidationParameters Parameters { get; }
}