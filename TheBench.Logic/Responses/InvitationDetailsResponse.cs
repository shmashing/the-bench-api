using TheBench.Logic.Models;

namespace TheBench.Logic.Responses;

public record InvitationDetailsResponse(
    string Id,
    string TeamId,
    string TeamName,
    string? TeamLogo,
    Sport TeamSport,
    string InviterId,
    string InviterName,
    string InviterEmail,
    string InviteeEmail,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    InvitationStatus Status,
    bool IsExpired
    );