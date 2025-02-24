namespace Ordering.Infrastructure.Identity;

public class KeycloakOptions
{
    public string BaseUrl { get; set; }
    public string Realm { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string GrantType { get; set; }
}