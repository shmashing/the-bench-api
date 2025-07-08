using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Database;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class TeamAdapter(TeamContext teamContext) : ITeamAdapter
{
    public async Task<Team> CreateTeam(Team team)
    {
        teamContext.Teams.Add(team);
        await teamContext.SaveChangesAsync();
        return team;
    }

    public async Task<Team?> GetTeam(string teamId)
    {
        return await teamContext.Teams.FindAsync(teamId);
    }

    public async Task<List<Team>> GetTeamsByMember(string userId)
    {
        return await teamContext.Teams
            .Where(t => t.MemberIds.Contains(userId))
            .ToListAsync();
    }

    public async Task<List<Team>> GetTeamsByManager(string userId)
    {
        return await teamContext.Teams
            .Where(t => t.ManagerIds.Contains(userId))
            .ToListAsync();
    }

    public async Task<Team> UpdateTeam(Team team)
    {
        teamContext.Teams.Update(team);
        await teamContext.SaveChangesAsync();
        return team;
    }

    public async Task<bool> AddUserToTeam(string teamId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) return false;
        
        if (!team.MemberIds.Contains(userId))
        {
            team.MemberIds.Add(userId);
            await teamContext.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> RemoveUserFromTeam(string teamId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) return false;
        
        if (team.MemberIds.Contains(userId))
        {
            team.MemberIds.Remove(userId);
            // Also remove user from managers if they were a manager
            if (team.ManagerIds.Contains(userId))
            {
                team.ManagerIds.Remove(userId);
            }
            await teamContext.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> AddManagerToTeam(string teamId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) return false;
        
        // Ensure user is a member first
        if (!team.MemberIds.Contains(userId))
        {
            team.MemberIds.Add(userId);
        }
        
        if (!team.ManagerIds.Contains(userId))
        {
            team.ManagerIds.Add(userId);
            await teamContext.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> RemoveManagerFromTeam(string teamId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) return false;
        
        if (team.ManagerIds.Contains(userId))
        {
            // Don't remove the last manager
            if (team.ManagerIds.Count <= 1)
            {
                return false;
            }
            
            team.ManagerIds.Remove(userId);
            await teamContext.SaveChangesAsync();
        }
        return true;
    }

    public async Task<TeamInvitation> CreateTeamInvitation(TeamInvitation invitation)
    {
        teamContext.TeamInvitations.Add(invitation);
        await teamContext.SaveChangesAsync();
        return invitation;
    }

    public async Task<TeamInvitation?> GetTeamInvitation(string invitationId)
    {
        return await teamContext.TeamInvitations.FindAsync(invitationId);
    }

    public async Task<List<TeamInvitation>> GetTeamInvitationsByTeam(string teamId)
    {
        return await teamContext.TeamInvitations
            .Where(i => i.TeamId == teamId && !i.IsAccepted)
            .ToListAsync();
    }

    public async Task<List<TeamInvitation>> GetTeamInvitationsByEmail(string email)
    {
        return await teamContext.TeamInvitations
            .Where(i => i.InviteeEmail == email && !i.IsAccepted)
            .ToListAsync();
    }

    public async Task<bool> MarkInvitationAsAccepted(string invitationId)
    {
        var invitation = await teamContext.TeamInvitations.FindAsync(invitationId);
        if (invitation == null) return false;
        
        invitation.MarkAccepted();
        await teamContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInvitation(string invitationId)
    {
        var invitation = await teamContext.TeamInvitations.FindAsync(invitationId);
        if (invitation == null) return false;
        
        teamContext.TeamInvitations.Remove(invitation);
        await teamContext.SaveChangesAsync();
        return true;
    }
}