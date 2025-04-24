using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Security;

public interface IUserContextService
{
    string GetUserId();
    string GetUserName();
}

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public string GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId ?? throw new UnauthorizedAccessException("User ID not found");
    }

    public string GetUserName()
    {
        var userName = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userName ?? throw new UnauthorizedAccessException("User ID not found");
    }
}