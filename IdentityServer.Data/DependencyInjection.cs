using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Core.Interfaces.Services;
using IdentityServer.Data.Configuration;
using IdentityServer.Data.Models;
using IdentityServer.Data.Persistence;
using IdentityServer.Data.Repositories;
using IdentityServer.Data.Seeding;
using IdentityServer.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.Data; 

public static class DependencyInjection {

    public static async Task<IServiceCollection> AddDataServicesAsync(this IServiceCollection services, IConfiguration config, bool isDevelopment) {

        services.AddDatabaseContext(config, isDevelopment);
        
        services.EnsureEncryptionKeys(config);

        services.AddDataRepositories();
        services.AddIdentityServices();
        services.AddAuthenticationServices();
        
        services.AddTransient<TokenFactory>();
        services.AddTransient<IUserOperationService, UserOperationService>();

        return services;
    }
    
    public static async Task EnsureDatabaseOnStartupAsync(this WebApplication app, bool isDevelopment) {
        using (var scope = app.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            if (isDevelopment) {
                await SeedDatabaseAsync(scope);
            }
        }
    }

    private static async Task SeedDatabaseAsync(IServiceScope scope) {
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeding>>();
        var seeding = new DatabaseSeeding(userRepo, roleRepo, logger);
        await seeding.SeedUsersAsync(15);
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

        services.AddTransient<IUserManager<ApplicationUser>, UserManager>();
        
        return services;
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services) {
        services.AddIdentity<ApplicationUser, ApplicationRole>(opt => {
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789- ";
            })
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