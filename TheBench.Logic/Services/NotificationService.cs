using TheBench.Logic.Models;
using TheBench.Logic.Responses;

namespace TheBench.Logic.Services;

public class NotificationService
{
    public Task SendEmail(string email, string subject, string message)
    {
        // In a real system, this would integrate with an email service
        Console.WriteLine($"Sending email to {email}, Subject: {subject}, Message: {message}");
        return Task.CompletedTask;
    }
    
    public Task SendNotification(string userId, string title, string message)
    {
        // In a real system, this would integrate with a push notification service
        Console.WriteLine($"Sending notification to user {userId}, Title: {title}, Message: {message}");
        return Task.CompletedTask;
    }

    public Task SendTeamInvites(List<TeamInvitation> invites)
    {
        Console.WriteLine($"Sending team invites for {invites.Count} users");
        return Task.CompletedTask;
    }
    
    public Task SendNewGameNotification(TeamMember organizer, TeamResponse team, Game game)
    {
        // In a real system, this would integrate with a push notification service
        Console.WriteLine($"{organizer.FirstName} has scheduled a game for {team.Name} against {game.OpponentTeamName}");
        return Task.CompletedTask;
    }
}