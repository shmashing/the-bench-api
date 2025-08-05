using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record SubstituteRequestResponse(
    string Id,
    string TeamId,
    string GameId,
    int NumberOfSubs,
    string[] Positions,
    string Location,
    DateTime Date,
    Sport Sport,
    string TeamName
    );