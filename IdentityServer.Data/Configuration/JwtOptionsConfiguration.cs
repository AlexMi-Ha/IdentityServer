using IdentityServer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdentityServer.Data.Configuration; 

public class JwtOptionsConfiguration : IConfigureNamedOptions<JwtBearerOptions> {

    private readonly IJwtHandler _jwtHandler;
    
    public JwtOptionsConfiguration(IJwtHandler jwtHandler) {
        _jwtHandler = jwtHandler;
    }
    
    public void Configure(JwtBearerOptions options) {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options) {
        if (name == JwtBearerDefaults.AuthenticationScheme) {
            options.TokenValidationParameters = _jwtHandler.Parameters;
            options.Events = new JwtBearerEvents() {
                OnMessageReceived = ctx => {
                    ctx.Token = ctx.Request.Cookies["identity-token"];
                    return Task.CompletedTask;
                }
            };
        }
    }
}