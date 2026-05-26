namespace RatePulse.Models.Firestore;

public class AdminLogModel
{
    public string Id { get; set; } = string.Empty;
    public string AdminUid { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetUid { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
