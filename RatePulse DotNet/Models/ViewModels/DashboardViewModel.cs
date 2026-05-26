namespace RatePulse.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int ActiveAlerts { get; set; }
    public int NotificationsSentToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public Dictionary<string, double> LatestRates { get; set; } = new();
    public DateTime LastRateFetch { get; set; }
    public List<TopCurrencyPair> TopCurrencyPairs { get; set; } = new();
    public List<DailyUserStat> DailyUserStats { get; set; } = new();
}

public class TopCurrencyPair
{
    public string Pair { get; set; } = string.Empty;
    public int AlertCount { get; set; }
}

public class DailyUserStat
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}
