namespace RatePulse.Models.Firestore;

public class UserModel
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public List<string> Permissions { get; set; } = new();
    public List<string> PreferredCurrencies { get; set; } = new() { "USD", "EUR", "TRY" };
    public bool IsBanned { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}
