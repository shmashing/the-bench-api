namespace TheBench.Logic.Requests.V1;

public record CreateSubstituteRequest(
    string TeamId,
    int NumberOfSubs,
    string[] Positions
    );
