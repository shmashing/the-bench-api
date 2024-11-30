using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Models;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(UserService userService) : Controller
{
    private UserService UserService { get; } = userService;

    [HttpGet]
    public IActionResult Initialize()
    {
        UserService.InitializeDatabase();
        return Ok();
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var user = await UserService.GetUserById(id);
            if (user == null) return NotFound();
                    
            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving user details: ${e.Message}");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User user)
    {
        try
        {
            var newUser = await UserService.CreateUser(user);
            return Ok(newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}