namespace TheBench.Logic.Requests.V1;

public record CreateGameRequest(
    string TeamId,
    string OpponentTeamName,
    DateTime StartTime,
    string Location,
    string? Notes = null
);