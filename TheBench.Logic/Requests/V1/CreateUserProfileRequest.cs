using TheBench.Logic.Models;

namespace TheBench.Logic.Requests.V1;

public record CreateUserProfileRequest(
    string Auth0Id,
    string FirstName,
    string LastName,
    string Email,
    Gender Gender,
    string? Avatar = null
    );