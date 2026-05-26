namespace RatePulse.Models.DTOs;

public class RegisterUserDto
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    public string DisplayName { get; set; } = string.Empty;
    public List<string> PreferredCurrencies { get; set; } = new();
}

public class UpdateFcmTokenDto
{
    public string FcmToken { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> PreferredCurrencies { get; set; } = new();
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AdminUpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}

public class BroadcastNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Target { get; set; } = "all";
    public string? TargetUid { get; set; }
}
