using System.Text.Json.Serialization;

namespace Ordering.Application.DTOs;

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}