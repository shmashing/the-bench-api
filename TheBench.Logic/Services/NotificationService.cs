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
}