using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class GameController(GameService gameService, TeamService teamService, NotificationService notificationService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        try
        {
            var team = await teamService.GetTeamById(request.TeamId);
            if (team == null) 
            {
                return NotFound("Team not found");
            }
            
            var game = await gameService.CreateGame(request);
            
            await notificationService.SendNewGameNotification(team.Members.First(), team, game);
            
            var response = GameResponse.FromGame(game, team.Name);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating game: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGame(string gameId)
    {
        try
        {
            var game = await gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found");
            }
            var team = await teamService.GetTeamById(game.TeamId);
            if (team == null)
            {
                return NotFound("Team not found");
            }
            
            return Ok(GameResponse.FromGame(game, team.Name));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving game");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("sub_requests")]
    public async Task<IActionResult> GetOpenSubRequests()
    {
        try
        {
            var openRequests = await gameService.GetOpenSubstituteRequests();
            return Ok(openRequests);
        } 
        catch(Exception ex)
        {
            Console.WriteLine($"Error retrieving open substitute requests: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{gameId}/subs")]
    public async Task<IActionResult> CreateSubstituteRequest([FromRoute] string gameId, [FromBody] CreateSubstituteRequest request)
    {
        try
        {
            var game = await gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found");
            }
            
            var result = await gameService.CreateSubstituteRequest(request, gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating substitute request: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{gameId}/subs")]
    public async Task<IActionResult> GetOpenSubRequestsForGame([FromRoute] string gameId)
    {
        try
        {
            var result = await gameService.GetOpenSubRequestsForGame(gameId);
            if (result == null)
            {
                return NotFound("No open substitute requests found for this game.");
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating substitute request: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
}