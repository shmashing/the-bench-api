using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using TheBench.Logic.Config;
using TheBench.Logic.Models;

namespace TheBench.Logic.Adapters;

public class NotificationAdapter(
    ApiConfiguration apiConfiguration,
    MailGunConfiguration mailGunConfiguration
    ) : INotificationAdapter
{
    public async Task<bool> SendTeamInvite(TeamInvitation invitation)
    {
        var options = new RestClientOptions(mailGunConfiguration.BaseUri)
        {
            Authenticator = new HttpBasicAuthenticator("api", mailGunConfiguration.ApiKey)
        };
        
        var client = new RestClient(options);
        var request = new RestRequest($"{mailGunConfiguration.Domain}/messages", Method.Post)
        {
            AlwaysMultipartFormData = true
        };
        
        var invitationLink = $"{apiConfiguration.BaseUiUri}/invitation?id={invitation.Id}&teamId={invitation.TeamId}";
        var emailVariables = new EmailVariables(
            invitationLink,
            invitation.InviterEmail,
            invitation.InviterName,
            invitationLink,
            invitation.TeamName,
            invitationLink
        );

        var emailVariablesJson = JsonSerializer.Serialize(emailVariables);
        Console.WriteLine(emailVariablesJson);
        
        request.AddParameter("from", "The Bench <postmaster@the-bench.us>");
        request.AddParameter("to", $"{invitation.InviteeEmail}");
        request.AddParameter("subject", "You're Invited to Join a Team on The Bench");
        request.AddParameter("template", "team invite template");
        request.AddParameter("h:X-Mailgun-Variables", emailVariablesJson);
        
        var response = await client.ExecuteAsync(request);
        
        if (response.IsSuccessful) return true;
        
        Console.WriteLine($"Error sending notification: {response.Content}");
        return false;
    }
}

public record EmailVariables(
    string INVITATION_LINK,
    string INVITER_EMAIL,
    string INVITER_NAME,
    string PRIVACY_POLICY_LINK,
    string TEAM_NAME,
    string TERMS_LINK
    );