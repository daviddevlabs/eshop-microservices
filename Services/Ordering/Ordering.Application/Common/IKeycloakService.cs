namespace Ordering.Application.Common;

public interface IKeycloakService
{
    Task<string> GetAdminTokenAsync();
    Task<KeycloakUser?> GetUserByUserIdAsync(Guid userId);
}