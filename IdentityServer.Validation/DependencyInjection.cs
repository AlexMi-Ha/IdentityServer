using FluentValidation;
using IdentityServer.Core.Dto;
using IdentityServer.Validation.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Validation; 

public static class DependencyInjection {

    public static IServiceCollection AddValidators(this IServiceCollection services) {

        services.AddTransient<IValidator<ChangeNameModel>, ChangeNameModelValidator>();
        services.AddTransient<IValidator<LoginModel>, LoginModelValidator>();
        services.AddTransient<IValidator<RegisterModel>, RegisterValidator>();
        services.AddTransient<IValidator<RoleModel>, RoleModelValidator>();
        
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        
        return services;
    } 
}