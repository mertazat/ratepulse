namespace RatePulse.Models.Firestore;

public class AlertModel
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public double TargetRate { get; set; }
    public string Direction { get; set; } = "above";
    public bool IsActive { get; set; } = true;
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
