using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Models;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PlayerController(PlayerService playerService) : Controller
{
    private PlayerService PlayerService { get; } = playerService;

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var player = await PlayerService.GetPlayerById(id);
        return Ok(player);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Player player)
    {
        var newPlayer = await PlayerService.CreatePlayer(player);
        return Ok(newPlayer);
    }
}