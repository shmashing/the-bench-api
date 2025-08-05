using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface IGameAdapter
{
    public Task<Game> CreateGame(Game game);
    public Task<Game?> GetGame(string gameId);
    public Task<List<Game>> GetGamesForTeam(string teamId);
    public Task<List<Game>> GetGamesForUser(string userId);
}