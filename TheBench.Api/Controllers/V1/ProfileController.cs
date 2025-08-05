using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfileController(
    UserService userService,
    GameService gameService,
    TeamService teamService
    ) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUserProfileRequest request)
    {
        Console.WriteLine($"Received POST request: {request}");
        try
        {
            var newUserProfile = await userService.CreateUserProfile(request);
            return Ok(newUserProfile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating new user: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId)
    {
        try
        {
            var userProfile = await userService.GetUserProfile(userId);
            
            if (userProfile == null) return NotFound();
            
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user profile: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> FindUsers([FromQuery(Name = "q")] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<UserProfileResponse>());
            }
            
            var users = await userService.FindUsers(query);
            
            return Ok(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching for users: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("auth/{authId}")]
    public async Task<IActionResult> GetUserByAuthId(string authId)
    {
        try
        {
            var userProfile = await userService.GetUserProfileByAuthId(authId);
            
            if (userProfile == null) return NotFound();
            
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user profile: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{userId}/games")]
    public async Task<IActionResult> GetUserGames(string userId)
    {
        try
        {
            var games = await gameService.GetGamesForUser(userId);
            var response = new List<GameResponse>();
            foreach (var game in games)
            {
                var team = await teamService.GetTeamById(game.TeamId);
                var gameResponse = GameResponse.FromGame(game, team!.Name);
                response.Add(gameResponse);
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user games: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
}