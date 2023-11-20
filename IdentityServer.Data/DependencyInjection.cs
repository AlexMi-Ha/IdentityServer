using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Configuration;
using IdentityServer.Data.Models;
using IdentityServer.Data.Persistence;
using IdentityServer.Data.Repositories;
using IdentityServer.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.Data; 

public static class DependencyInjection {

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config, bool isDevelopment) {

        services.AddDatabaseContext(config, isDevelopment);
        
        services.EnsureEncryptionKeys(config);

        services.AddDataRepositories();
        services.AddIdentityServices();
        services.AddAuthenticationServices();
        
        services.AddTransient<TokenFactory>();
        services.AddTransient<IUserOperationService, UserOperationService>();
        
        
        return services;
    }
    
    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, bool isDevelopment) {
        if (isDevelopment) {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            );
        }
        else {
            /*services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection")!,
                    new MariaDbServerVersion(new Version(configuration["DatabaseConfig:Version"]!)))
            ); TODO configure*/
            throw new NotImplementedException();
        }
        return services;
    }
    
    private static IServiceCollection AddDataRepositories(this IServiceCollection services) {

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserImageRepository, UserImageRepository>();
        services.AddTransient<IRoleRepository, RoleRepository>();

        return services;
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services) {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }
    
    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services) {
        services.AddSingleton<IJwtHandler, JwtHandler>();

        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null);

        // Add Jwt configuration after initialization
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtOptionsConfiguration>();

        return services;
    }
    
}