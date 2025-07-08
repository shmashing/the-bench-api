namespace TheBench.Logic.Responses;

public record TeamInvitationResponse(
    string Id,
    string TeamId,
    string InviterId,
    string InviteeEmail,
    bool IsAccepted,
    string TeamName
);