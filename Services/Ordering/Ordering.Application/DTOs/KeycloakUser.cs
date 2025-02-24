using System.Text.Json.Serialization;

namespace Ordering.Application.DTOs;

public class KeycloakUser
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
}