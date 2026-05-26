using Microsoft.AspNetCore.Mvc;
using RatePulse.Filters;
using RatePulse.Models.ViewModels;
using RatePulse.Repositories;

namespace RatePulse.Controllers.Admin;

[Route("admin/dashboard")]
[RequirePermission]
public class AdminDashboardController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IRateRepository _rateRepository;
    private readonly IAlertRepository _alertRepository;

    public AdminDashboardController(
        IUserRepository userRepository,
        IRateRepository rateRepository,
        IAlertRepository alertRepository)
    {
        _userRepository = userRepository;
        _rateRepository = rateRepository;
        _alertRepository = alertRepository;
    }

    [HttpGet("")]
    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        var latestRate = await _rateRepository.GetLatestAsync();
        var allAlerts = await _alertRepository.GetAllAsync();

        var vm = new DashboardViewModel
        {
            TotalUsers = users.Count,
            NewUsersThisWeek = users.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
            ActiveAlerts = allAlerts.Count(a => a.IsActive),
            LatestRates = latestRate?.Values ?? new(),
            LastRateFetch = latestRate?.FetchedAt ?? DateTime.MinValue,
            TopCurrencyPairs = allAlerts
                .GroupBy(a => $"{a.FromCurrency}/{a.ToCurrency}")
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new TopCurrencyPair { Pair = g.Key, AlertCount = g.Count() })
                .ToList(),
            DailyUserStats = Enumerable.Range(0, 7)
                .Select(i => DateTime.UtcNow.AddDays(-6 + i))
                .Select(d => new DailyUserStat
                {
                    Date = d.ToString("dd/MM"),
                    Count = users.Count(u => u.CreatedAt.Date == d.Date)
                }).ToList()
        };

        return View(vm);
    }
}
