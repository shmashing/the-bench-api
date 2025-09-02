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

    public async Task<CreateTeamInvitationResponse> InviteUsersToTeam(string teamId, string inviterId, List<string> inviteeEmails)
    {
        // Verify inviter is authorized to invite users
        var team = await _teamAdapter.GetTeam(teamId);
        if (team == null) throw new Exception("Team not found");
        if (!team.ManagerIds.Contains(inviterId)) throw new Exception("User is not authorized to invite members to this team");

        var inviter = await _userAdapter.GetUserProfile(inviterId);
        if (inviter == null) throw new Exception("Inviter not found");
        
        var invitations = new List<TeamInvitation>();
        foreach (var inviteeEmail in inviteeEmails)
        {
            var userHasOpenInvite = await _teamAdapter.UserHasOpenInviteForTeam(teamId, inviteeEmail);
            if (userHasOpenInvite) continue;
            
            var invitationId = _idService.Generate("invitation");
            var invitation = new TeamInvitation(
                invitationId, 
                teamId, 
                team.Name, 
                inviterId, 
                inviter.FirstName + " " + inviter.LastName, 
                inviter.Email, 
                inviteeEmail
                );
            invitations.Add(invitation);
        }
        
        var createdInvitations = await _teamAdapter.CreateTeamInvitations(invitations);

        // Send email notification
        await notificationService.SendTeamInvites(createdInvitations);
            
        return new CreateTeamInvitationResponse(createdInvitations);
    }
    
    public async Task<InvitationDetailsResponse> GetTeamInvitation(string teamId, string invitationId)
    {
        var invitation = await _teamAdapter.GetTeamInvitation(invitationId);
        
        if (invitation == null) throw new Exception("Invitation not found");
        if (invitation.TeamId != teamId) throw new Exception("Invitation does not belong to the specified team");
        
        var team = await _teamAdapter.GetTeam(teamId);
        
        if (team == null) throw new Exception("Team not found");
        
        var invitationExpired = invitation.ExpiresAt < DateTime.UtcNow;
        var response = new InvitationDetailsResponse(
            invitation.Id,
            team.Id,
            team.Name,
            team.Logo,
            team.Sport,
            invitation.InviterId,
            invitation.InviterName,
            invitation.InviterEmail,
            invitation.InviteeEmail,
            invitation.CreatedAt,
            invitation.ExpiresAt,
            invitation.Status,
            invitationExpired
        );
        
        return response;
    }

    public async Task<TeamResponse> AcceptTeamInvitation(string teamId, string invitationId, string userId)
    {
        var invitation = await _teamAdapter.GetTeamInvitation(invitationId);
        ValidateInvitationRequest(invitation, teamId);
        
        var result = await _teamAdapter.AddUserToTeam(invitation!.TeamId, userId);
        if (!result) throw new Exception("Failed to add user to team");
        
        await _teamAdapter.UpdateInvitationStatus(invitationId, InvitationStatus.Accepted);
        
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
    
    public async Task<TeamResponse> DeclineTeamInvitation(string teamId, string invitationId, string userId)
    {
        var invitation = await _teamAdapter.GetTeamInvitation(invitationId);
        ValidateInvitationRequest(invitation, teamId);
        
        await _teamAdapter.UpdateInvitationStatus(invitationId, InvitationStatus.Declined);
        Console.WriteLine("Invitation declined for user: " + userId);
        var team = await _teamAdapter.GetTeam(invitation!.TeamId) 
                   ?? throw new Exception("Team not found");
        
        Console.WriteLine("Notifying team managers of declined invitation");
        // Notify team managers
        foreach (var managerId in team.ManagerIds)
        {
            var manager = await _userAdapter.GetUserProfile(managerId);
            if (manager != null)
            {
                // In a real system, we'd get the user's details to include in the notification
                await notificationService.SendNotification(managerId, "Team Member Declined", 
                    $"A member has declined your invitation to join your team {team.Name}");
            }
        }
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(team, members);
    }

    public async Task<TeamResponse> AddManagerToTeam(string teamId, string currentManagerId, string newManagerId)
    {
        var team = await _teamAdapter.AddManagerToTeam(teamId, currentManagerId, newManagerId);
        
        // Notify the new manager
        await notificationService.SendNotification(newManagerId, "Team Manager Role", 
            $"You have been made a manager of team: {team.Name}");
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);        
        return GetTeamResponse(team, members);
    }
    
    public async Task<TeamResponse> DemoteManagerToPlayer(string teamId, string currentManagerId, string managerToRemoveId)
    {
        var team = await _teamAdapter.DemoteManagerToPlayer(teamId, currentManagerId, managerToRemoveId);
        
        // Notify the removed manager
        await notificationService.SendNotification(managerToRemoveId, "Team Manager Role Removed", 
            $"Your manager role for team {team.Name} has been removed");
        
        var managers = await _userAdapter.GetUsers(team.ManagerIds);
        var players = await _userAdapter.GetUsers(team.MemberIds);
        
        var members = GetTeamMembers(managers, players);
        return GetTeamResponse(team, members);
    }
    
    public async Task<TeamResponse> RemoveMemberFromTeam(string teamId, string managerId, string memberToRemoveId)
    {
        var updatedTeam = await _teamAdapter.RemoveUserFromTeam(teamId, memberToRemoveId);
        
        // Notify the removed member
        await notificationService.SendNotification(memberToRemoveId, "Removed from Team", 
            $"You have been removed from the team {updatedTeam.Name}");
        
        var managers = await _userAdapter.GetUsers(updatedTeam.ManagerIds);
        var players = await _userAdapter.GetUsers(updatedTeam.MemberIds);
        
        var members = GetTeamMembers(managers, players);
        return GetTeamResponse(updatedTeam, members);
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
    
    public async Task<List<UserTeamInvitationsResponse>> GetUserTeamInvitations(string userId, string? status)
    {
        var user = await _userAdapter.GetUserProfile(userId);
        if (user == null) throw new Exception("User not found");
        
        var invitations = await _teamAdapter.GetTeamInvitationsByEmail(user.Email, status);
        var response = new List<UserTeamInvitationsResponse>();
        foreach (var invite in invitations)
        {
            var team = await _teamAdapter.GetTeam(invite.TeamId);
            if (team == null) continue;
            
            response.Add(new UserTeamInvitationsResponse(
                invite.Id,
                team.Id,
                team.Name,
                team.Logo,
                team.Sport,
                invite.InviterId,
                invite.InviterName,
                invite.CreatedAt,
                invite.Status
                ));
        }
        
        return response;
    }
    
    public async Task<CreateTeamInvitationResponse> GetPendingInvitations(string email)
    {
        var invitations = await _teamAdapter.GetTeamInvitationsByEmail(email);
        var responses = new CreateTeamInvitationResponse(invitations);
        
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
    
    private static void ValidateInvitationRequest(TeamInvitation? invitation, string teamId)
    {
        if (invitation == null) throw new Exception("Invitation not found");
        if (invitation.TeamId != teamId) throw new Exception("Invitation does not belong to the specified team");
        if (invitation.Status == InvitationStatus.Expired ||  invitation.ExpiresAt < DateTime.UtcNow) throw new Exception("Invitation has expired");
        if (invitation.Status != InvitationStatus.Pending) throw new Exception("Invitation has already been responded to");
    }
}