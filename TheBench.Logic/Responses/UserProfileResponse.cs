using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record UserProfileResponse(
    string Id,
    string UserId,
    string Location,
    Gender Gender,
    List<Sport> Sports,
    Dictionary<string, Availability> Schedule
    );