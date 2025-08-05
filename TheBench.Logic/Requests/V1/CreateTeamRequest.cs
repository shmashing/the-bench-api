using TheBench.Logic.Models;

namespace TheBench.Logic.Requests.V1;

public record CreateTeamRequest(
    string ManagerId, 
    string TeamName,
    Sport Sport,
    string Description,
    string? Logo
);