namespace TheBench.Logic.Models;

public class TeamInvitation(
    string id,
    string teamId,
    string inviterId,
    string inviteeEmail,
    bool isAccepted
)
{
    public string Id { get; init; } = id;
    public string TeamId { get; init; } = teamId;
    public string InviterId { get; init; } = inviterId;
    public string InviteeEmail { get; init; } = inviteeEmail;
    public bool IsAccepted { get; set; } = isAccepted;

    public void MarkAccepted()
    {
        IsAccepted = true;
    }
};