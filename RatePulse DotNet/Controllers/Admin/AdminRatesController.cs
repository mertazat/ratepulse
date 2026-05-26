using Microsoft.AspNetCore.Mvc;
using RatePulse.Filters;
using RatePulse.Repositories;
using RatePulse.Services.Currency;

namespace RatePulse.Controllers.Admin;

[Route("admin/rates")]
[RequirePermission("manage_rates")]
public class AdminRatesController : Controller
{
    private readonly IRateRepository _rateRepository;
    private readonly ICurrencyFetchService _currencyFetchService;

    public AdminRatesController(IRateRepository rateRepository, ICurrencyFetchService currencyFetchService)
    {
        _rateRepository = rateRepository;
        _currencyFetchService = currencyFetchService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var latest = await _rateRepository.GetLatestAsync();
        var history = await _rateRepository.GetHistoryAsync(48);
        ViewBag.Latest = latest;
        ViewBag.History = history;
        ViewBag.HistoryChronological = history.OrderBy(r => r.FetchedAt).ToList();
        return View();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        await _currencyFetchService.FetchAndSaveLatestRatesAsync();
        TempData["Success"] = "Kurlar güncellendi.";
        return RedirectToAction("Index");
    }
}
