namespace TheBench.Logic.Models;

public record Team(
    string Id,
    string Name,
    string FounderId,
    List<string> ManagerIds,
    List<string> MemberIds,
    Sport Sport,
    string Description,
    string? Logo = null
    );