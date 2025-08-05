using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record TeamInvitationResponse(
    List<TeamInvitation> Invitations
);