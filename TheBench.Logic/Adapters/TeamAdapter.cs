using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Database;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class TeamAdapter(UserContext teamContext) : ITeamAdapter
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

    public Task<bool> DeleteTeam(Team team)
    {
        teamContext.Teams.Remove(team);
        return teamContext.SaveChangesAsync().ContinueWith(t => t.Result > 0);
    }

    public async Task<List<Team>> GetTeamsByMember(string userId)
    {
        var teams = await teamContext.Teams.ToListAsync();
        return teams.Where(t => t.ManagerIds.Contains(userId) || t.MemberIds.Contains(userId)).ToList();
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
        Console.WriteLine($"adding user {userId} to team {teamId}");
        var team = await teamContext.Teams.AsTracking().FirstOrDefaultAsync(t => t.Id == teamId);
        if (team == null) return false;
        if (team.MemberIds.Contains(userId)) return false;

        Console.WriteLine("Found team, checking membership");
        
        var memberIds = team.MemberIds;
        memberIds.Add(userId);
        team.MemberIds = memberIds;
        teamContext.Entry(team).Property(t => t.MemberIds).IsModified = true;
        
        await teamContext.SaveChangesAsync();
        
        Console.WriteLine("User added to team!");
        return true;
    }

    public async Task<Team> RemoveUserFromTeam(string teamId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.MemberIds.Contains(userId) && !team.ManagerIds.Contains(userId)) 
            throw new Exception("User is not a member of the team");

        var members = team.MemberIds;
        members.Remove(userId);
        
        var managers = team.ManagerIds;
        managers.Remove(userId);
        
        team.MemberIds = members;
        team.ManagerIds = managers;
        teamContext.Entry(team).Property(t => t.MemberIds).IsModified = true;
        teamContext.Entry(team).Property(t => t.ManagerIds).IsModified = true;

        await teamContext.SaveChangesAsync();
        
        return team;
    }

    public async Task<Team> AddManagerToTeam(string teamId, string managerId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(managerId)) throw new Exception("Only a manager can promote a member to manager");
        if (team.ManagerIds.Contains(userId)) return team;
        if (!team.MemberIds.Contains(userId)) throw new Exception("User must be a member of the team to be promoted to manager");
        
        var managers = team.ManagerIds;
        managers.Add(userId);
        
        var members = team.MemberIds;
        members.Remove(userId);
        
        team.MemberIds = members;
        team.ManagerIds = managers;
        teamContext.Entry(team).Property(t => t.MemberIds).IsModified = true;
        teamContext.Entry(team).Property(t => t.ManagerIds).IsModified = true;

        await teamContext.SaveChangesAsync();
        
        return team;
    }

    public async Task<Team> DemoteManagerToPlayer(string teamId, string managerId, string userId)
    {
        var team = await teamContext.Teams.FindAsync(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(managerId)) throw new Exception("Only a manager can demote a manager to player");
        if (!team.ManagerIds.Contains(userId)) return team;
        if (managerId == userId) throw new Exception("A manager cannot demote themselves to player");
        if (team.ManagerIds.Count <= 1) throw new Exception("Cannot demote the last manager of the team");
        if (team.MemberIds.Contains(userId)) return team;
        
        var members = team.MemberIds;
        members.Add(userId);
        
        var managers = team.ManagerIds;
        managers.Remove(userId);
        
        team.MemberIds = members;
        team.ManagerIds = managers;
        teamContext.Entry(team).Property(t => t.MemberIds).IsModified = true;
        teamContext.Entry(team).Property(t => t.ManagerIds).IsModified = true;
        await teamContext.SaveChangesAsync();
        
        return team;
    }

    public async Task<List<TeamInvitation>> CreateTeamInvitations(List<TeamInvitation> invitations)
    {
        teamContext.TeamInvitations.AddRange(invitations);
        await teamContext.SaveChangesAsync();
        
        return invitations;
    }

    public async Task<TeamInvitation?> GetTeamInvitation(string invitationId)
    {
        return await teamContext.TeamInvitations.FindAsync(invitationId);
    }

    public async Task<List<TeamInvitation>> GetTeamInvitationsByTeam(string teamId)
    {
        return await teamContext.TeamInvitations
            .Where(i => i.TeamId == teamId && i.Status == InvitationStatus.Pending)
            .ToListAsync();
    }

    public async Task<List<TeamInvitation>> GetTeamInvitationsByEmail(string email, string? status = null)
    {
        if (status == null)
        {
            return await teamContext.TeamInvitations
                .Where(i => i.InviteeEmail == email)
                .ToListAsync();
        }
        
        return await teamContext.TeamInvitations
            .Where(i => i.InviteeEmail == email && i.Status.ToString() == status)
            .ToListAsync();
    }
    
    public async Task<bool> UserHasOpenInviteForTeam(string teamId, string email)
    {
        return await teamContext.TeamInvitations
            .AnyAsync(i => i.TeamId == teamId && i.InviteeEmail == email && i.Status == InvitationStatus.Pending);
    }

    public async Task<bool> UpdateInvitationStatus(string invitationId, InvitationStatus newStatus)
    {
        var invitation = await teamContext.TeamInvitations.FindAsync(invitationId);
        if (invitation == null) return false;
        
        invitation.SetStatus(newStatus);
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