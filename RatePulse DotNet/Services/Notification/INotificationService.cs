namespace RatePulse.Services.Notification;

public interface INotificationService
{
    Task SendToUserAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
    Task BroadcastAsync(string title, string body);
}
