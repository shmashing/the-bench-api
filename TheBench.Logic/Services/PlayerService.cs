using TheBench.Logic.Models;

namespace TheBench.Logic.Services;

public class PlayerService
{
    public async Task<Player> GetPlayerById(string playerId)
    {
        return new Player("bob", "ross", playerId);
    }

    public async Task<Player> CreatePlayer(Player player)
    {
        return new Player("bob", "ross", player.Id);
    }
}