using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class TeamController(TeamService teamService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        try
        {
            var team = await teamService.CreateTeam(request.ManagerId, request.TeamName);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTeams(string userId)
    {
        try
        {
            var teams = await teamService.GetTeamsForUser(userId);
            return Ok(teams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("managed/{userId}")]
    public async Task<IActionResult> GetManagedTeams(string userId)
    {
        try
        {
            var teams = await teamService.GetManagedTeams(userId);
            return Ok(teams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/invite")]
    public async Task<IActionResult> InviteToTeam(string teamId, [FromQuery] string inviterId, [FromBody] TeamInvitationRequest request)
    {
        try
        {
            var invitation = await teamService.InviteUserToTeam(teamId, inviterId, request.InviteeEmail);
            return Ok(invitation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("invitation/{invitationId}/accept")]
    public async Task<IActionResult> AcceptInvitation(string invitationId, [FromQuery] string userId)
    {
        try
        {
            var team = await teamService.AcceptTeamInvitation(invitationId, userId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/managers/add")]
    public async Task<IActionResult> AddManager(string teamId, [FromQuery] string currentManagerId, [FromQuery] string newManagerId)
    {
        try
        {
            var team = await teamService.AddManagerToTeam(teamId, currentManagerId, newManagerId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/managers/remove")]
    public async Task<IActionResult> RemoveManager(string teamId, [FromQuery] string currentManagerId, [FromQuery] string managerToRemoveId)
    {
        try
        {
            var team = await teamService.RemoveManagerFromTeam(teamId, currentManagerId, managerToRemoveId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("invitations")]
    public async Task<IActionResult> GetPendingInvitations([FromQuery] string email)
    {
        try
        {
            var invitations = await teamService.GetPendingInvitations(email);
            return Ok(invitations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}