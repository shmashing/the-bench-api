using Microsoft.AspNetCore.Mvc;
using TheBench.Api.Models;
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

            var userDto = new UserDto(user);
            
            return Ok(userDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving user details: ${e.Message}");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AuthenticatedUser authenticatedUser)
    {
        Console.WriteLine($"Received POST request: {authenticatedUser}");
        try
        {
            var newUser = await UserService.CreateUser(authenticatedUser);
            var userDto = new UserDto(newUser);
            Console.WriteLine($"New user created: {userDto.Name} - {userDto.Id}");
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating new user: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }
}