namespace TheBench.Logic.Responses;

public record TeamResponse(
    string Id,
    string Name,
    List<string> ManagerIds,
    List<string> MemberIds
);