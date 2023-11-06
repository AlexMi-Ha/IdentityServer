using System.Text;
using IdentityServer.Data.Extensions;
using IdentityServer.Data.Interfaces.Repositories;
using IdentityServer.Data.Models;
using IdentityServer.Data.Persistence;
using IdentityServer.Data.Repositories;
using IdentityServer.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Data; 

public static class DependencyInjection {

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config, bool isDevelopment) {

        services.AddDatabaseContext(config, isDevelopment);
        
        services.EnsureEncryptionKeys(config);

        services.AddDataRepositories();
        services.AddIdentityServices();
        
        services.AddTransient<TokenFactory>();
        
        
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

        return services;
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services) {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }
    
    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.TokenValidationParameters = new TokenValidationParameters {
                ValidIssuer = configuration.GetJwtIssuer(),
                ValidAudience = configuration.GetJwtAudience(),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        return services;
    }
    
}