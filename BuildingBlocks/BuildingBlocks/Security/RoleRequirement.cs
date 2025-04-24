using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Security;

public enum UserRole
{
    Admin,
    Employee,
    Customer
}

public class RoleRequirement(UserRole role) : IAuthorizationRequirement
{
    public UserRole Role { get; } = role;
}

public class RoleRequirementHandler(IConfiguration config) : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {         
        var userRoles = context.User.Claims.FirstOrDefault(c => c.Type == "resource_access")?.Value;
        if (userRoles is null) return Task.CompletedTask;
   
        var resourceRoles = JsonNode.Parse(userRoles);

        var roles = resourceRoles?[config["Authentication:ClientId"]!]?["roles"]?.AsArray();
        if (roles is null) return Task.CompletedTask;
        
        var role = roles.FirstOrDefault(x => 
            string.Equals(x?.ToString(), requirement.Role.ToString(), StringComparison.OrdinalIgnoreCase));
        
        if(role is not null) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}