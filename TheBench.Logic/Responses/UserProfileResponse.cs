using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record UserProfileResponse(
    string Id,
    string Auth0Id,
    string FirstName,
    string LastName,
    string Email,
    Gender Gender
    );