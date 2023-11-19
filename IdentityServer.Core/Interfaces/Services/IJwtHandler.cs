using System.Security.Claims;
using IdentityServer.Core.Dto;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Core.Interfaces.Services; 

public interface IJwtHandler {
    
    JwtModel Create(List<Claim> userClaims);
    TokenValidationParameters Parameters { get; }
}