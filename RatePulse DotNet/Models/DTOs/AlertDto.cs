namespace RatePulse.Models.DTOs;

public class CreateAlertDto
{
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public double TargetRate { get; set; }
    public string Direction { get; set; } = "above";
}

public class AlertResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public double TargetRate { get; set; }
    public string Direction { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
