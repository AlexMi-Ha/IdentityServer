using IdentityServer.Data.Interfaces.Repositories;
using IdentityServer.Data.Repositories;
using IdentityServer.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Data; 

public static class DependencyInjection {

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config) {
        
        services.EnsureEncryptionKeys(config);

        services.AddTransient<TokenFactory>();
        services.AddTransient<IUserRepository, UserRepository>();
        
        
        return services;
    }
    
    
}