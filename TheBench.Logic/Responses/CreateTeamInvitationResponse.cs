using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record CreateTeamInvitationResponse(
    List<TeamInvitation> Invitations
);