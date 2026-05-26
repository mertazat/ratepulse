namespace RatePulse.Models.DTOs;

public class LatestRatesDto
{
    public string Base { get; set; } = "USD";
    public DateTime FetchedAt { get; set; }
    public Dictionary<string, double> Rates { get; set; } = new();
}

public class RateHistoryDto
{
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public List<RateHistoryPoint> History { get; set; } = new();
}

public class RateHistoryPoint
{
    public DateTime Timestamp { get; set; }
    public double Rate { get; set; }
}

public class ManualRateUpdateDto
{
    public Dictionary<string, double> Values { get; set; } = new();
}
