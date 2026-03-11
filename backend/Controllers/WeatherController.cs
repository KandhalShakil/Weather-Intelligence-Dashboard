using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetCurrentWeather(string city)
    {
        return await ExecuteAsync(() => _weatherService.GetCurrentWeatherAsync(city), city);
    }

    [HttpGet("forecast/{city}")]
    public async Task<IActionResult> GetForecast(string city)
    {
        return await ExecuteAsync(() => _weatherService.GetForecastAsync(city), city);
    }

    [HttpGet("hourly/{city}")]
    public async Task<IActionResult> GetHourlyWeather(string city)
    {
        return await ExecuteAsync(() => _weatherService.GetHourlyWeatherAsync(city), city);
    }

    [HttpGet("airquality/{city}")]
    public async Task<IActionResult> GetAirQuality(string city)
    {
        return await ExecuteAsync(() => _weatherService.GetAirQualityAsync(city), city);
    }

    [HttpGet("location")]
    public async Task<IActionResult> GetCurrentLocationWeather([FromQuery] double lat, [FromQuery] double lon)
    {
        if (lat is < -90 or > 90 || lon is < -180 or > 180)
        {
            return BadRequest(new { error = "Latitude or longitude is out of range." });
        }

        try
        {
            var weather = await _weatherService.GetCurrentWeatherByCoordinatesAsync(lat, lon);
            return Ok(weather);
        }
        catch (WeatherServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { error = ex.Message });
        }
    }

    [HttpGet("location/ip")]
    public async Task<IActionResult> GetCurrentLocationWeatherFromIp()
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherFromIpAsync();
            return Ok(weather);
        }
        catch (WeatherServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { error = ex.Message });
        }
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(Array.Empty<string>());
        }

        try
        {
            var matches = await _weatherService.SearchCitiesAsync(query);
            return Ok(matches);
        }
        catch (WeatherServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { error = ex.Message });
        }
    }

    private async Task<IActionResult> ExecuteAsync<T>(Func<Task<T?>> action, string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest(new { error = "City name is required." });
        }

        try
        {
            var result = await action();
            return Ok(result);
        }
        catch (WeatherServiceException ex)
        {
            return StatusCode(ex.StatusCode, new { error = ex.Message });
        }
    }
}

