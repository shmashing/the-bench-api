namespace TheBench.Logic.Models;

public record UserProfile(
    string Id,
    string Auth0Id,
    string FirstName,
    string LastName,
    string Email,
    Gender Gender,
    string? Avatar = null
    );