using Microsoft.AspNetCore.Mvc;
using RatePulse.Filters;
using RatePulse.Repositories;

namespace RatePulse.Controllers.Admin;

[Route("admin/alerts")]
[RequirePermission("manage_alerts")]
public class AdminAlertsController : Controller
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUserRepository _userRepository;

    public AdminAlertsController(IAlertRepository alertRepository, IUserRepository userRepository)
    {
        _alertRepository = alertRepository;
        _userRepository = userRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] bool? active)
    {
        var alerts = await _alertRepository.GetAllAsync();

        if (active.HasValue)
            alerts = alerts.Where(a => a.IsActive == active.Value).ToList();

        var users = await _userRepository.GetAllAsync();
        var userDict = users.ToDictionary(u => u.Uid, u => u.Email);

        ViewBag.Alerts = alerts;
        ViewBag.UserDict = userDict;
        ViewBag.ActiveFilter = active;

        return View();
    }
}
