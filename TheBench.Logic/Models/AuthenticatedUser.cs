using System.Text.Json.Serialization;

namespace TheBench.Logic.Models;

public class AuthenticatedUser
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("phone_number")]
    public required string PhoneNumber { get; set; }
    
    [JsonPropertyName("city")]
    public required string City { get; set; }
    
    [JsonPropertyName("sports")]
    public required List<Sport> Sports { get; set; }
}