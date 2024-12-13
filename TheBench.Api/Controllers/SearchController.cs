using Microsoft.AspNetCore.Mvc;
using TheBench.Api.Models;
using TheBench.Logic.Models;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SearchController(SearchService searchService) : Controller
{
    [HttpGet]
    public IActionResult Search([FromQuery] string sport, [FromQuery] string day, [FromQuery] string time)
    {
        try
        {
            var users = searchService.FindUsers(sport, day, time);
            var userDtos = users.ConvertAll(u => new UserDto(u));
            
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, ex.Message);
        }
    }
}