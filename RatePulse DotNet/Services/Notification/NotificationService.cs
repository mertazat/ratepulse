using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;

namespace RatePulse.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly IUserRepository _userRepository;
    private readonly FirestoreDb _db;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUserRepository userRepository,
        FirestoreDb db,
        ILogger<NotificationService> logger)
    {
        _userRepository = userRepository;
        _db = db;
        _logger = logger;
    }

    public async Task SendToUserAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrEmpty(fcmToken)) return;

        try
        {
            var message = new Message
            {
                Token = fcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            await LogNotificationAsync("alert", fcmToken, title, body, true);
            _logger.LogInformation("Push sent. MessageId: {Id}", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification to token {Token}", fcmToken);
            await LogNotificationAsync("alert", fcmToken, title, body, false);
        }
    }

    public async Task BroadcastAsync(string title, string body)
    {
        var users = await _userRepository.GetAllAsync();
        var tokensToSend = users
            .Where(u => !string.IsNullOrEmpty(u.FcmToken) && !u.IsBanned)
            .Select(u => u.FcmToken)
            .Distinct()
            .ToList();

        if (tokensToSend.Count == 0) return;

        var messages = tokensToSend.Select(token => new Message
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            }
        }).ToList();

        try
        {
            var batchResponse = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);
            await LogNotificationAsync("broadcast", "all", title, body, batchResponse.FailureCount == 0);
            _logger.LogInformation("Broadcast sent. Success: {S}, Fail: {F}", batchResponse.SuccessCount, batchResponse.FailureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Broadcast failed");
        }
    }

    private async Task LogNotificationAsync(string type, string sentTo, string title, string body, bool success)
    {
        var log = new Dictionary<string, object>
        {
            ["type"] = type,
            ["sentTo"] = sentTo,
            ["title"] = title,
            ["body"] = body,
            ["success"] = success,
            ["sentAt"] = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        await _db.Collection("notifications").AddAsync(log);
    }
}
