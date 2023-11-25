using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Data.Services; 

internal class JwtHandler : IJwtHandler {

    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _expireTimeSpan;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    private SecurityKey _issuerSigningKey;
    private SigningCredentials _signingCredentials;

    private RSA _privateRsa;
    private RSA _publicRsa;
    
    public TokenValidationParameters Parameters { get; private set; }
    
    public JwtHandler(IConfiguration config) {
        _issuer = config.GetJwtIssuer();
        _audience = config.GetJwtAudience();
        _expireTimeSpan = TimeSpan.FromHours(12);
        
        ConfigureRsa(config.GetJwtPublicKeyPath(), config.GetJwtPrivateKeyPath());
        ConfigureJwtParameters();
    }

    ~JwtHandler() {
        _privateRsa?.Dispose();
        _publicRsa?.Dispose();
    }

    private void ConfigureRsa(string publicKeyPath, string privateKeyPath) {
        _publicRsa = RSA.Create();
        var publicKeyXml = File.ReadAllText(publicKeyPath);
        _publicRsa.FromXmlString(publicKeyXml);
        _issuerSigningKey = new RsaSecurityKey(_publicRsa);

        _privateRsa = RSA.Create();
        var privateKeyXml = File.ReadAllText(privateKeyPath);
        _privateRsa.FromXmlString(privateKeyXml);
        var privateKey = new RsaSecurityKey(_privateRsa);
        _signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
    }

    private void ConfigureJwtParameters() {
        Parameters = new TokenValidationParameters {
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _issuerSigningKey,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    }

    public JwtModel Create(List<Claim> userClaims) {
        var expiresDateTime = DateTimeOffset.Now.AddHours(12);
        long issuedAt = EpochTime.GetIntDate(DateTime.Now);
        long expires = EpochTime.GetIntDate(expiresDateTime.DateTime);
        userClaims.Add(new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToString()));
        userClaims.Add(new Claim(JwtRegisteredClaimNames.Exp, expires.ToString()));

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.Now.Add(_expireTimeSpan),
            claims: userClaims,
            signingCredentials: _signingCredentials
        );
        var token = _jwtSecurityTokenHandler.WriteToken(jwt);

        return new JwtModel {
            Token = token,
            ExpiresEpoch = expires,
            ExpiresDateTimeOffset = expiresDateTime
        };
    }

}