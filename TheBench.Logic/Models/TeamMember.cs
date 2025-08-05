namespace TheBench.Logic.Models;

public record TeamMember(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    TeamRole Role,
    Gender Gender,
    string? Avatar
    );