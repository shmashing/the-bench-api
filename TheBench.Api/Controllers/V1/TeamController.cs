using Microsoft.AspNetCore.Mvc;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;
using TheBench.Logic.Services;

namespace TheBench.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class TeamController(TeamService teamService, GameService gameService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        try
        {
            var team = await teamService.CreateTeam(
                request.ManagerId, 
                request.TeamName,
                request.Sport,
                request.Description,
                request.Logo
                );
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamById(string teamId)
    {
        try
        {
            var team = await teamService.GetTeamById(teamId);
            if (team == null)
            {
                return NotFound("Team not found");
            }
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpDelete("{teamId}")]
    public async Task<IActionResult> DeleteTeam(string teamId)
    {
        try
        {
            var result = await teamService.DeleteTeam(teamId);
            if (result)
            {
                return Ok("Team deleted successfully");
            }
            return NotFound("Team not found");
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
    
    [HttpGet("{teamId}/games")]
    public async Task<IActionResult> GetGamesForTeam(string teamId)
    {
        try
        {
            var games = await gameService.GetGamesForTeam(teamId);
            var team = await teamService.GetTeamById(teamId);
            
            var response = new List<GameResponse>();
            foreach (var game in games)
            {
                response.Add(GameResponse.FromGame(game, team!.Name));
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/invite")]
    public async Task<IActionResult> InviteToTeam(string teamId, [FromBody] TeamInvitationRequest request)
    {
        try
        {
            var invitation = await teamService.InviteUsersToTeam(teamId, request.InviterId, request.UserEmails);
            return Ok(invitation);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inviting users to team: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{teamId}/invitations/{invitationId}")]
    public async Task<IActionResult> GetTeamInvitation(string teamId, string invitationId)
    {
        try
        {
            var invitation = await teamService.GetTeamInvitation(teamId, invitationId);
            return Ok(invitation);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving team invitation: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/invitations/{invitationId}/accept")]
    public async Task<IActionResult> AcceptInvitation(string teamId, string invitationId, [FromQuery] string userId)
    {
        if (userId.Trim() == "")
        {
            return BadRequest("User ID is required");
        }
        
        try
        {
            var team = await teamService.AcceptTeamInvitation(teamId, invitationId, userId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/invitations/{invitationId}/decline")]
    public async Task<IActionResult> DeclineInvitation(string teamId, string invitationId, [FromQuery] string userId)
    {
        if (userId.Trim() == "")
        {
            return BadRequest("User ID is required");
        }
        
        try
        {
            var team = await teamService.DeclineTeamInvitation(teamId, invitationId, userId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/managers/add/{memberId}")]
    public async Task<IActionResult> AddManager(string teamId, [FromQuery] string managerId, string memberId)
    {
        try
        {
            var team = await teamService.AddManagerToTeam(teamId, managerId, memberId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("{teamId}/managers/remove/{memberId}")]
    public async Task<IActionResult> DemoteManagerToPlayer(string teamId, [FromQuery] string managerId,  string memberId)
    {
        try
        {
            var team = await teamService.DemoteManagerToPlayer(teamId, managerId, memberId);
            return Ok(team);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpDelete("{teamId}/members/{memberId}")]
    public async Task<IActionResult> RemoveTeamMember(string teamId, [FromQuery] string managerId, string memberId)
    {
        try
        {
            var team = await teamService.RemoveMemberFromTeam(teamId, managerId, memberId);
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