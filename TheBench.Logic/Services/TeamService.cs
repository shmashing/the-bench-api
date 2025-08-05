using TheBench.Logic.Adapters;
using TheBench.Logic.Models;
using TheBench.Logic.Responses;

namespace TheBench.Logic.Services;

public class TeamService(ITeamAdapter teamAdapter, IUserAdapter userAdapter, NotificationService notificationService)
{
    private readonly IUserAdapter _userAdapter = userAdapter;
    private readonly ITeamAdapter _teamAdapter = teamAdapter;
    private readonly IdService _idService = new();
    
    public async Task<TeamResponse> CreateTeam(
        string founderId, 
        string name,
        Sport sport,
        string description,
        string? logo = null
        )
    {
        var teamId = _idService.Generate("team");
        var team = new Team(
            teamId, 
            name, 
            founderId, 
            [founderId], 
            [], 
            sport, 
            description, 
            logo
            );
        var createdTeam = await _teamAdapter.CreateTeam(team);
        
        var managers = await _userAdapter.GetUsers(createdTeam.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(createdTeam, members);
    }
    
    public async Task<bool> DeleteTeam(string teamId)
    {
        var team = await _teamAdapter.GetTeam(teamId);
        if (team == null) return false;

        // Remove all invitations related to this team
        var invitations = await _teamAdapter.GetTeamInvitationsByTeam(teamId);
        foreach (var invitation in invitations)
        {
            await _teamAdapter.DeleteInvitation(invitation.Id);
        }

        // Remove the team itself
        return await _teamAdapter.DeleteTeam(team);
    }

    public async Task<TeamResponse?> GetTeamById(string teamId)
    {
        var team = await _teamAdapter.GetTeam(teamId);

        if (team == null) return null;
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(team, members);
    }

    public async Task<TeamInvitationResponse> InviteUsersToTeam(string teamId, string inviterId, List<string> inviteeEmails)
    {
        // Verify inviter is authorized to invite users
        var team = await _teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(inviterId)) throw new Exception("User is not authorized to invite members to this team");
        
        var invitations = new List<TeamInvitation>();
        inviteeEmails.ForEach(inviteeEmail =>
        {
            var invitationId = _idService.Generate("invitation");
            var invitation = new TeamInvitation(invitationId, teamId, inviterId, inviteeEmail, false);
            invitations.Add(invitation);
        });
        
        var createdInvitations = await _teamAdapter.CreateTeamInvitations(invitations);

        // Send email notification
        await notificationService.SendTeamInvites(createdInvitations);
            
        return new TeamInvitationResponse(createdInvitations);
    }

    public async Task<TeamResponse> AcceptTeamInvitation(string invitationId, string userId)
    {
        var invitation = await _teamAdapter.GetTeamInvitation(invitationId);
        if (invitation == null || invitation.IsAccepted) throw new Exception("Invalid invitation");

        await _teamAdapter.AddUserToTeam(invitation.TeamId, userId);
        await _teamAdapter.MarkInvitationAsAccepted(invitationId);
        
        var team = await _teamAdapter.GetTeam(invitation.TeamId) 
            ?? throw new Exception("Team not found");
            
        // Notify team managers
        foreach (var managerId in team.ManagerIds)
        {
            var manager = await _userAdapter.GetUserProfile(managerId);
            if (manager != null)
            {
                // In a real system, we'd get the user's details to include in the notification
                await notificationService.SendNotification(managerId, "Team Member Added", 
                    $"A new member has joined your team {team.Name}");
            }
        }
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(team, members);
    }

    public async Task<TeamResponse> AddManagerToTeam(string teamId, string currentManagerId, string newManagerId)
    {
        var team = await _teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(currentManagerId)) throw new Exception("User is not authorized to update team managers");
        
        await _teamAdapter.AddManagerToTeam(teamId, newManagerId);
        team = await _teamAdapter.GetTeam(teamId) ?? throw new Exception("Team not found");
        
        // Notify the new manager
        await notificationService.SendNotification(newManagerId, "Team Manager Role", 
            $"You have been made a manager of team: {team.Name}");
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(team, members);
    }
    
    public async Task<TeamResponse> RemoveManagerFromTeam(string teamId, string currentManagerId, string managerToRemoveId)
    {
        var team = await _teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(currentManagerId)) throw new Exception("User is not authorized to update team managers");
        
        if (team.ManagerIds.Count <= 1)
            throw new Exception("Cannot remove the only manager from the team");
            
        var success = await _teamAdapter.RemoveManagerFromTeam(teamId, managerToRemoveId);
        if (!success) throw new Exception("Failed to remove manager role");
        
        team = await _teamAdapter.GetTeam(teamId) ?? throw new Exception("Team not found");
        
        // Notify the removed manager
        await notificationService.SendNotification(managerToRemoveId, "Team Manager Role Removed", 
            $"Your manager role for team {team.Name} has been removed");
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);
        return GetTeamResponse(team, members);
    }
    
    public async Task<List<TeamResponse>> GetTeamsForUser(string userId)
    {
        var teams = await _teamAdapter.GetTeamsByMember(userId);
        var response = new List<TeamResponse>();
        foreach (var team in teams)
        {
            var managers = await _userAdapter.GetUsers(team.ManagerIds);
            var players = await _userAdapter.GetUsers(team.MemberIds);
            
            var members = GetTeamMembers(managers, players);
            var teamResponse = GetTeamResponse(team, members);
            response.Add(teamResponse);
        }
        
        return response;
    }
    
    public async Task<List<TeamResponse>> GetManagedTeams(string userId)
    {
        var teams = await _teamAdapter.GetTeamsByManager(userId);
        var response = new List<TeamResponse>();
        foreach (var team in teams)
        {
            var managers = await _userAdapter.GetUsers(team.ManagerIds);
            var players = await _userAdapter.GetUsers(team.MemberIds);
            
            var members = GetTeamMembers(managers, players);
            var teamResponse = GetTeamResponse(team, members);
            response.Add(teamResponse);
        }
        
        return response;
    }
    
    public async Task<TeamInvitationResponse> GetPendingInvitations(string email)
    {
        var invitations = await _teamAdapter.GetTeamInvitationsByEmail(email);
        var responses = new TeamInvitationResponse(invitations);
        
        return responses;
    }

    private static List<TeamMember> GetTeamMembers(List<UserProfile> managers, List<UserProfile> players)
    {
        var teamMembers = managers.ConvertAll(player => 
            new TeamMember(
                player.Id,
                player.FirstName,
                player.LastName, 
                player.Email, 
                TeamRole.Captain, 
                player.Gender, 
                player.Avatar
                )
        );
        
        teamMembers.AddRange(players.ConvertAll(
            player => new TeamMember(
                player.Id, 
                player.FirstName,
                player.LastName,
                player.Email,
                TeamRole.Player,
                player.Gender,
                player.Avatar)
        ));

        return teamMembers;
    }
    
    private static TeamResponse GetTeamResponse(Team team, List<TeamMember> members)
    {
        return new TeamResponse(
            team.Id,
            team.Name,
            members,
            team.Sport,
            team.Description,
            team.Logo
        );
    }
}