using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Database;
using TheBench.Logic.Models;
using TheBench.Logic.Services;

namespace TheBench.Logic.Adapters;

public class GameAdapter(UserContext dbContext) : IGameAdapter
{
    public async Task<Game> CreateGame(Game game)
    {
        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();
        
        return game;
    }

    public async Task<Game?> GetGame(string gameId)
    {
        return await dbContext.Games.FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<List<Game>> GetGamesForTeam(string teamId)
    {
        return await dbContext.Games.Where(game => game.TeamId == teamId).ToListAsync();
    }

    public async Task<List<Game>> GetGamesForUser(string userId)
    {
        var teams = await dbContext.Teams.ToListAsync();
        var usersTeams = teams
            .Where(t => t.ManagerIds.Contains(userId) || t.MemberIds.Contains(userId))
            .Select(t => t.Id)
            .ToList();
        
        return await dbContext
            .Games
            .Where(g => usersTeams.Contains(g.TeamId))
            .ToListAsync();
    }
}