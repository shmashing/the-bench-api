using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record TeamResponse(
    string Id,
    string Name,
    List<TeamMember> Members,
    Sport Sport,
    string Description,
    string? Logo = null
);