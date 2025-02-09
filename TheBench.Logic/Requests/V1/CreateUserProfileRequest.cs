using TheBench.Logic.Models;

namespace TheBench.Logic.Requests.V1;

public record CreateUserProfileRequest(
    string UserId,
    string Location,
    Gender Gender,
    List<Sport> Sports,
    Schedule? Schedule
    );