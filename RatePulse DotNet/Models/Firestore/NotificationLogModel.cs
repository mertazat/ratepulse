namespace RatePulse.Models.Firestore;

public class NotificationLogModel
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = "alert";
    public string SentTo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
