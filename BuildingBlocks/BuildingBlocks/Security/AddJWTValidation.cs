using System.Security.Claims;
using System.Text.Json.Nodes;
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
        var authConfig = config.GetSection("Authentication").Get<AuthenticationConfig>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = authConfig!.Authority;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity) return Task.CompletedTask;
                    
                    var resourceAccessClaim = context.Principal?.FindFirst("resource_access");
                    if (resourceAccessClaim is null) return Task.CompletedTask;
                        
                    var resourceAccess = JsonNode.Parse(resourceAccessClaim.Value);
                        
                    var roles = resourceAccess?[authConfig.ClientId]?["roles"]?.AsArray();
                    if (roles is null) return Task.CompletedTask;
                    
                    foreach (var role in roles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role?.ToString()!));
                    }
                    return Task.CompletedTask;
                }
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthorizationWithRoles(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy =>
                policy.Requirements.Add(new RoleRequirement(UserRole.Admin)))
            .AddPolicy("EmployeePolicy", policy => 
                policy.Requirements.Add(new RoleRequirement(UserRole.Employee)))
            .AddPolicy("CustomerPolicy", policy =>
                policy.Requirements.Add(new RoleRequirement(UserRole.Customer)));
        
        services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
        
        return services;
    }
}