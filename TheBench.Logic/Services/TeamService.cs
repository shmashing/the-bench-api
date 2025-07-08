using TheBench.Logic.Adapters;
using TheBench.Logic.Models;
using TheBench.Logic.Requests.V1;
using TheBench.Logic.Responses;

namespace TheBench.Logic.Services;

public class TeamService(ITeamAdapter teamAdapter, IUserAdapter userAdapter, NotificationService notificationService)
{
    private readonly IdService idService = new();
    
    public async Task<TeamResponse> CreateTeam(string founderId, string name)
    {
        var teamId = idService.Generate("team");
        var team = new Team(teamId, name, founderId, [founderId], [founderId]);
        var createdTeam = await teamAdapter.CreateTeam(team);
        return GetTeamResponse(createdTeam);
    }

    public async Task<TeamInvitationResponse> InviteUserToTeam(string teamId, string inviterId, string inviteeEmail)
    {
        // Verify inviter is authorized to invite users
        var team = await teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(inviterId)) throw new Exception("User is not authorized to invite members to this team");
        
        var invitationId = idService.Generate("invitation");
        var invitation = new TeamInvitation(invitationId, teamId, inviterId, inviteeEmail, false);
        var createdInvitation = await teamAdapter.CreateTeamInvitation(invitation);

        // Send email notification
        await notificationService.SendEmail(inviteeEmail, "Team Invitation", 
            $"You have been invited to join the team: {team.Name}. Click here to accept the invitation.");
            
        return new TeamInvitationResponse(
            createdInvitation.Id,
            createdInvitation.TeamId,
            createdInvitation.InviterId,
            createdInvitation.InviteeEmail,
            createdInvitation.IsAccepted,
            team.Name);
    }

    public async Task<TeamResponse> AcceptTeamInvitation(string invitationId, string userId)
    {
        var invitation = await teamAdapter.GetTeamInvitation(invitationId);
        if (invitation == null || invitation.IsAccepted) throw new Exception("Invalid invitation");

        await teamAdapter.AddUserToTeam(invitation.TeamId, userId);
        await teamAdapter.MarkInvitationAsAccepted(invitationId);
        
        var team = await teamAdapter.GetTeam(invitation.TeamId) 
            ?? throw new Exception("Team not found");
            
        // Notify team managers
        foreach (var managerId in team.ManagerIds)
        {
            var manager = await userAdapter.GetUserProfile(managerId);
            if (manager != null)
            {
                // In a real system, we'd get the user's details to include in the notification
                await notificationService.SendNotification(managerId, "Team Member Added", 
                    $"A new member has joined your team {team.Name}");
            }
        }
        
        return GetTeamResponse(team);
    }

    public async Task<TeamResponse> AddManagerToTeam(string teamId, string currentManagerId, string newManagerId)
    {
        var team = await teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(currentManagerId)) throw new Exception("User is not authorized to update team managers");
        
        await teamAdapter.AddManagerToTeam(teamId, newManagerId);
        team = await teamAdapter.GetTeam(teamId) ?? throw new Exception("Team not found");
        
        // Notify the new manager
        await notificationService.SendNotification(newManagerId, "Team Manager Role", 
            $"You have been made a manager of team: {team.Name}");
            
        return GetTeamResponse(team);
    }
    
    public async Task<TeamResponse> RemoveManagerFromTeam(string teamId, string currentManagerId, string managerToRemoveId)
    {
        var team = await teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(currentManagerId)) throw new Exception("User is not authorized to update team managers");
        
        if (team.ManagerIds.Count <= 1)
            throw new Exception("Cannot remove the only manager from the team");
            
        var success = await teamAdapter.RemoveManagerFromTeam(teamId, managerToRemoveId);
        if (!success) throw new Exception("Failed to remove manager role");
        
        team = await teamAdapter.GetTeam(teamId) ?? throw new Exception("Team not found");
        
        // Notify the removed manager
        await notificationService.SendNotification(managerToRemoveId, "Team Manager Role Removed", 
            $"Your manager role for team {team.Name} has been removed");
            
        return GetTeamResponse(team);
    }
    
    public async Task<List<TeamResponse>> GetTeamsForUser(string userId)
    {
        var teams = await teamAdapter.GetTeamsByMember(userId);
        return teams.Select(GetTeamResponse).ToList();
    }
    
    public async Task<List<TeamResponse>> GetManagedTeams(string userId)
    {
        var teams = await teamAdapter.GetTeamsByManager(userId);
        return teams.Select(GetTeamResponse).ToList();
    }
    
    public async Task<List<TeamInvitationResponse>> GetPendingInvitations(string email)
    {
        var invitations = await teamAdapter.GetTeamInvitationsByEmail(email);
        var responses = new List<TeamInvitationResponse>();
        
        foreach (var invitation in invitations)
        {
            var team = await teamAdapter.GetTeam(invitation.TeamId);
            if (team != null)
            {
                responses.Add(new TeamInvitationResponse(
                    invitation.Id,
                    invitation.TeamId,
                    invitation.InviterId,
                    invitation.InviteeEmail,
                    invitation.IsAccepted,
                    team.Name));
            }
        }
        
        return responses;
    }
    
    private static TeamResponse GetTeamResponse(Team team)
    {
        return new TeamResponse(
            team.Id,
            team.Name,
            team.ManagerIds,
            team.MemberIds
        );
    }
}