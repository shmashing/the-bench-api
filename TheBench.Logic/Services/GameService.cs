using TheBench.Logic.Adapters;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;

namespace TheBench.Logic.Services;

public class GameService(
    IGameAdapter gameAdapter, 
    ITeamAdapter teamAdapter, 
    ISubstituteRequestAdapter substituteRequestAdapter, 
    IdService idService)
{
    public async Task<Game> CreateGame(CreateGameRequest request)
    {
        var gameId = idService.Generate("game");
        var game = new Game(
            gameId,
            request.TeamId,
            request.OpponentTeamName,
            request.StartTime,
            request.Location,
            // [],
            request.Notes);

        return await gameAdapter.CreateGame(game);
    }
    
    public async Task<Game?> GetGame(string gameId)
    {
        return await gameAdapter.GetGame(gameId);
    }
    
    public async Task<List<Game>> GetGamesForTeam(string teamId)
    {
        return await gameAdapter.GetGamesForTeam(teamId);
    }

    public async Task<List<Game>> GetGamesForUser(string userId)
    {
        var usersGames = await gameAdapter.GetGamesForUser(userId);
        return usersGames;
    }

    public async Task<SubstituteRequestResponse> CreateSubstituteRequest(CreateSubstituteRequest request, string gameId)
    {
        var requestId = idService.Generate("sub_request");
        var game = await gameAdapter.GetGame(gameId);
        if (game == null)
        {
            throw new ArgumentException($"Game with ID {gameId} does not exist.");
        }

        var team = await teamAdapter.GetTeam(request.TeamId);
        if (team == null)
        {
            throw new ArgumentException($"Team with ID {request.TeamId} does not exist.");
        }

        var substituteRequest = new SubstituteRequest(
            requestId,
            game.Id,
            team.Id,
            request.NumberOfSubs,
            request.Positions,
            game.Location,
            game.StartTime,
            team.Name,
            team.Sport
        );

        var newRequest = await substituteRequestAdapter.CreateSubstituteRequest(substituteRequest)!;
        
        return new SubstituteRequestResponse(
            newRequest.Id,
            newRequest.TeamId,
            newRequest.GameId,
            newRequest.NumberOfSubs,
            newRequest.Positions,
            newRequest.Location,
            newRequest.StartTime,
            newRequest.Sport,
            newRequest.TeamName
        );
    }
    
    public async Task<List<SubstituteRequestResponse>> GetOpenSubstituteRequests()
    {
        var openRequests = await substituteRequestAdapter.GetOpenSubstituteRequests();
        var response = openRequests.Select(r => new SubstituteRequestResponse(
            r.Id,
            r.TeamId,
            r.GameId,
            r.NumberOfSubs,
            r.Positions,
            r.Location,
            r.StartTime,
            r.Sport,
            r.TeamName
        )).ToList();

        return response;
    }

    public async Task<SubstituteRequestResponse?> GetOpenSubRequestsForGame(string gameId)
    {
        var openRequest = await substituteRequestAdapter.GetOpenSubRequestsForGame(gameId);
        if (openRequest == null) return null;
        
        return new SubstituteRequestResponse
        (
            openRequest.Id,
            openRequest.TeamId,
            openRequest.GameId,
            openRequest.NumberOfSubs,
            openRequest.Positions,
            openRequest.Location,
            openRequest.StartTime,
            openRequest.Sport,
            openRequest.TeamName
        );
    }
}