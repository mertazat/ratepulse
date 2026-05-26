using Microsoft.AspNetCore.Mvc;
using RatePulse.Models.DTOs;
using RatePulse.Repositories;
using RatePulse.Services.Currency;

namespace RatePulse.Controllers.API;

[ApiController]
[Route("api/rates")]
public class RatesController : ControllerBase
{
    private readonly IRateRepository _rateRepository;
    private readonly ICurrencyFetchService _currencyFetchService;

    public RatesController(IRateRepository rateRepository, ICurrencyFetchService currencyFetchService)
    {
        _rateRepository = rateRepository;
        _currencyFetchService = currencyFetchService;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest()
    {
        var rate = await _rateRepository.GetLatestAsync();
        if (rate == null)
            return NotFound(ApiResponse.Fail("No rate data available"));

        return Ok(ApiResponse<LatestRatesDto>.Ok(new LatestRatesDto
        {
            Base = rate.Base,
            FetchedAt = rate.FetchedAt,
            Rates = rate.Values
        }));
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string from = "USD", [FromQuery] string to = "TRY", [FromQuery] int limit = 24)
    {
        var history = await _rateRepository.GetHistoryAsync(limit);

        var points = history
            .Where(r => r.Values.ContainsKey(to) && (from == "USD" || r.Values.ContainsKey(from)))
            .Select(r =>
            {
                double rate = from == "USD"
                    ? r.Values[to]
                    : r.Values[to] / r.Values[from];

                return new RateHistoryPoint { Timestamp = r.FetchedAt, Rate = rate };
            })
            .OrderBy(p => p.Timestamp)
            .ToList();

        return Ok(ApiResponse<RateHistoryDto>.Ok(new RateHistoryDto
        {
            FromCurrency = from,
            ToCurrency = to,
            History = points
        }));
    }
}
