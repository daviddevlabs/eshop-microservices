namespace BuildingBlocks.Security;

public class AuthenticationConfig
{
  public string Authority { get; set; } = string.Empty;
  public string ValidIssuer { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string Audience { get; set; } = string.Empty;
}