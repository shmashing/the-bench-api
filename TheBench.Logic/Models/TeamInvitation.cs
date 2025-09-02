namespace TheBench.Logic.Models;

public class TeamInvitation(
    string id,
    string teamId,
    string teamName,
    string inviterId,
    string inviterName,
    string inviterEmail,
    string inviteeEmail,
    InvitationStatus status = InvitationStatus.Pending
)
{
    public string Id { get; init; } = id;
    public string TeamId { get; init; } = teamId;
    public string TeamName { get; init; } = teamName;
    public string InviterId { get; init; } = inviterId;
    public string InviterName { get; init; } = inviterName;
    public string InviterEmail { get; init; } = inviterEmail;
    public string InviteeEmail { get; init; } = inviteeEmail;
    public InvitationStatus Status { get; set; } = status;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddDays(7);

    public void SetStatus(InvitationStatus newStatus)
    {
        Status = newStatus;
    }
    
};

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined,
    Expired
}