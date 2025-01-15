using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Security;

public static class AddJwtValidation
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = config["Authentication:Authority"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config["Authentication:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = config["Authentication:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthorizationWithRoles(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy =>
                policy.Requirements.Add(new RoleRequirement(UserRole.Admin)))
            .AddPolicy("Employee", policy => 
                policy.Requirements.Add(new RoleRequirement(UserRole.Employee)))
            .AddPolicy("Customer", policy =>
                policy.Requirements.Add(new RoleRequirement(UserRole.Customer)));
        
        services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
        
        return services;
    }
}