using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using RatePulse.Filters;
using RatePulse.Models.DTOs;
using RatePulse.Services.Notification;

namespace RatePulse.Controllers.Admin;

[Route("admin/notifications")]
[RequirePermission("send_broadcast")]
public class AdminNotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly FirestoreDb _db;

    public AdminNotificationController(INotificationService notificationService, FirestoreDb db)
    {
        _notificationService = notificationService;
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var snapshot = await _db.Collection("notifications")
            .OrderByDescending("sentAt")
            .Limit(50)
            .GetSnapshotAsync();

        var logs = snapshot.Documents.Select(d => new
        {
            Id = d.Id,
            Type = d.ContainsField("type") ? d.GetValue<string>("type") : "",
            SentTo = d.ContainsField("sentTo") ? d.GetValue<string>("sentTo") : "",
            Title = d.ContainsField("title") ? d.GetValue<string>("title") : "",
            Body = d.ContainsField("body") ? d.GetValue<string>("body") : "",
            Success = d.ContainsField("success") && d.GetValue<bool>("success"),
            SentAt = d.ContainsField("sentAt") ? d.GetValue<Timestamp>("sentAt").ToDateTime() : DateTime.MinValue
        }).ToList();

        ViewBag.Logs = logs;
        return View();
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromForm] BroadcastNotificationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Body))
        {
            TempData["Error"] = "Başlık ve mesaj zorunludur.";
            return RedirectToAction("Index");
        }

        await _notificationService.BroadcastAsync(dto.Title, dto.Body);
        TempData["Success"] = "Bildirim başarıyla gönderildi.";
        return RedirectToAction("Index");
    }
}
