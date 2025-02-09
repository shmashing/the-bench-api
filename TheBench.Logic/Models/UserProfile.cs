namespace TheBench.Logic.Models;

public record UserProfile(
    string Id,
    string UserId,
    string Location,
    Gender Gender,
    List<Sport> Sports,
    Schedule Schedule
    );