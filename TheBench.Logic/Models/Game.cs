namespace TheBench.Logic.Models;

public record Game(
    string Id,
    string TeamId,
    string OpponentTeamName,
    DateTime StartTime,
    string Location,
    // List<Rsvp> Rsvps,
    string? Notes = null
    );