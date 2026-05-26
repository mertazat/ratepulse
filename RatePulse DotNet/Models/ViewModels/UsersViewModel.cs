using RatePulse.Models.Firestore;

namespace RatePulse.Models.ViewModels;

public class UsersViewModel
{
    public List<UserModel> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public string? SearchQuery { get; set; }
    public string? RoleFilter { get; set; }
}

public class UserDetailViewModel
{
    public UserModel User { get; set; } = new();
    public List<AlertModel> Alerts { get; set; } = new();
    public int TriggeredAlertsCount { get; set; }
}
