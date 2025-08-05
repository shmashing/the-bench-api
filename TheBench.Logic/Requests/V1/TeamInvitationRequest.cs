namespace TheBench.Logic.Requests.V1;

public record TeamInvitationRequest(
    string InviterId,
    List<string> UserEmails
    );