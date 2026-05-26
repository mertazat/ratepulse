using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using RatePulse.Filters;
using RatePulse.Models.DTOs;
using RatePulse.Models.ViewModels;
using RatePulse.Repositories;

namespace RatePulse.Controllers.Admin;

[Route("admin/users")]
[RequirePermission("view_users")]
public class AdminUsersController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly FirestoreDb _db;

    public AdminUsersController(IUserRepository userRepository, IAlertRepository alertRepository, FirestoreDb db)
    {
        _userRepository = userRepository;
        _alertRepository = alertRepository;
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] string? role)
    {
        var users = await _userRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
            users = users.Where(u =>
                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(role))
            users = users.Where(u => u.Role == role).ToList();

        return View(new UsersViewModel
        {
            Users = users,
            TotalCount = users.Count,
            SearchQuery = search,
            RoleFilter = role
        });
    }

    [HttpGet("{uid}")]
    public async Task<IActionResult> Detail(string uid)
    {
        var user = await _userRepository.GetByUidAsync(uid);
        if (user == null) return NotFound();

        var alerts = await _alertRepository.GetByUserIdAsync(uid);

        return View(new UserDetailViewModel
        {
            User = user,
            Alerts = alerts,
            TriggeredAlertsCount = alerts.Count(a => !a.IsActive && a.TriggeredAt.HasValue)
        });
    }

    [HttpPost("{uid}/role")]
    [RequirePermission("manage_admins")]
    public async Task<IActionResult> ChangeRole(string uid, [FromForm] string role)
    {
        var validRoles = new[] { "user", "moderator", "admin", "superadmin" };
        if (!validRoles.Contains(role))
            return BadRequest();

        await _userRepository.UpdateRoleAsync(uid, role);
        await LogAdminActionAsync("change_role", uid, $"Role changed to {role}");

        TempData["Success"] = "Rol güncellendi.";
        return RedirectToAction("Detail", new { uid });
    }

    [HttpPost("{uid}/ban")]
    public async Task<IActionResult> Ban(string uid)
    {
        var user = await _userRepository.GetByUidAsync(uid);
        if (user == null) return NotFound();

        bool newStatus = !user.IsBanned;
        await _userRepository.UpdateBanStatusAsync(uid, newStatus);
        await LogAdminActionAsync(newStatus ? "ban_user" : "unban_user", uid, $"User {(newStatus ? "banned" : "unbanned")}");

        TempData["Success"] = newStatus ? "Kullanıcı askıya alındı." : "Kullanıcının askısı kaldırıldı.";
        return RedirectToAction("Detail", new { uid });
    }

    [HttpPost("{uid}/delete")]
    [RequirePermission("manage_admins")]
    public async Task<IActionResult> Delete(string uid)
    {
        await _userRepository.DeleteAsync(uid);
        await LogAdminActionAsync("delete_user", uid, "User deleted");

        TempData["Success"] = "Kullanıcı silindi.";
        return RedirectToAction("Index");
    }

    private async Task LogAdminActionAsync(string action, string targetUid, string details)
    {
        var adminUid = HttpContext.Session.GetString("AdminUid") ?? "";
        var adminEmail = HttpContext.Session.GetString("AdminEmail") ?? "";

        await _db.Collection("admin_logs").AddAsync(new Dictionary<string, object>
        {
            ["adminUid"] = adminUid,
            ["adminEmail"] = adminEmail,
            ["action"] = action,
            ["targetUid"] = targetUid,
            ["details"] = details,
            ["timestamp"] = Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }
}
