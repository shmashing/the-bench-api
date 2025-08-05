using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public interface ITeamAdapter
{
    Task<Team> CreateTeam(Team team);
    Task<Team?> GetTeam(string teamId);
    Task<bool> DeleteTeam(Team team);
    Task<List<Team>> GetTeamsByMember(string userId);
    Task<List<Team>> GetTeamsByManager(string userId);
    Task<Team> UpdateTeam(Team team);
    Task<bool> AddUserToTeam(string teamId, string userId);
    Task<bool> RemoveUserFromTeam(string teamId, string userId);
    Task<bool> AddManagerToTeam(string teamId, string userId);
    Task<bool> RemoveManagerFromTeam(string teamId, string userId);
    
    Task<List<TeamInvitation>> CreateTeamInvitations(List<TeamInvitation> invitation);
    Task<TeamInvitation?> GetTeamInvitation(string invitationId);
    Task<List<TeamInvitation>> GetTeamInvitationsByTeam(string teamId);
    Task<List<TeamInvitation>> GetTeamInvitationsByEmail(string email);
    Task<bool> MarkInvitationAsAccepted(string invitationId);
    Task<bool> DeleteInvitation(string invitationId);
}