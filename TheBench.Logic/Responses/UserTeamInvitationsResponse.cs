using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record UserTeamInvitationsResponse(
    string Id,
    string TeamId,
    string TeamName,
    string? TeamLogo,
    Sport Sport,
    string InviterId,
    string InviterName,
    DateTime InvitedAt,
    InvitationStatus Status
    );