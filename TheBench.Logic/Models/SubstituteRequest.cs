namespace TheBench.Logic.Models;

public record SubstituteRequest(
    string Id,
    string GameId,
    string TeamId,
    int NumberOfSubs,
    string[] Positions,
    string Location,
    DateTime StartTime,
    string TeamName,
    Sport Sport
    );