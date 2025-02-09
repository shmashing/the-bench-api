using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers.V2;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfileController(UserService userService) : Controller
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
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user profile: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
}