using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public class GameResponse(
    string id,
    string teamId,
    string teamName,
    string opponentTeamName,
    DateTime startTime,
    string location,
    GameStatus status,
    List<Rsvp> rsvps,
    string? notes = null
)
{
    public string Id { get; } = id;
    public string TeamId { get; } = teamId;
    public string TeamName { get; } = teamName;
    public string OpponentTeamName { get; } = opponentTeamName;
    public DateTime StartTime { get; } = startTime;
    public string Location { get; } = location;
    public GameStatus Status { get; } = status;
    public List<Rsvp> Rsvps { get; } = rsvps;
    public string? Notes { get; } = notes;
    
    public static GameResponse FromGame(Game game, string teamName)
    {
        var gameStatus = game.StartTime < DateTime.UtcNow
            ? GameStatus.Completed
            : GameStatus.Upcoming;
        
        Console.WriteLine(gameStatus);
        return new GameResponse(
            game.Id,
            game.TeamId,
            teamName,
            game.OpponentTeamName,
            game.StartTime,
            game.Location,
            gameStatus,
            [],
            game.Notes
        );
    }
};