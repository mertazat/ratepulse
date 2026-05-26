namespace RatePulse.Models.Firestore;

public class RateModel
{
    public string Id { get; set; } = string.Empty;
    public string Base { get; set; } = "USD";
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, double> Values { get; set; } = new();
}
