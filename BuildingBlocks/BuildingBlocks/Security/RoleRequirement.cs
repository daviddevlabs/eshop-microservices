using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;

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

public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var realmRoles = context.User.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
        if (realmRoles == null) return Task.CompletedTask;
   
        var resourceRolesJson = JsonNode.Parse(realmRoles);
        if (resourceRolesJson?["roles"] is not JsonArray roles) return Task.CompletedTask;
        
        var role = roles.FirstOrDefault(x => 
            string.Equals(x?.ToString(), requirement.Role.ToString(), StringComparison.CurrentCultureIgnoreCase));
        
        if(role != null) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}