using System.Net;
using System.Text.Json;
using backend.Models;

namespace backend.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private const string BaseUrl = "https://api.openweathermap.org";
    private const string IpApiUrl = "http://ip-api.com/json/";
private static readonly string[] PopularCities =
{
    "Ahmedabad, Gujarat, IN",
    "Bangalore, Karnataka, IN",
    "Chennai, Tamil Nadu, IN",
    "Delhi, IN",
    "Hyderabad, Telangana, IN",
    "Jaipur, Rajasthan, IN",
    "Kolkata, West Bengal, IN",
    "Mumbai, Maharashtra, IN",
    "Pune, Maharashtra, IN",
    "Surat, Gujarat, IN",

    "Amsterdam, North Holland, NL",
    "Athens, GR",
    "Barcelona, Catalonia, ES",
    "Berlin, DE",
    "Brussels, BE",
    "Budapest, HU",
    "Copenhagen, DK",
    "Dublin, IE",
    "Lisbon, PT",
    "London, GB",
    "Madrid, ES",
    "Milan, IT",
    "Munich, DE",
    "Paris, FR",
    "Prague, CZ",
    "Rome, IT",
    "Stockholm, SE",
    "Vienna, AT",
    "Warsaw, PL",
    "Zurich, CH",

    "Bangkok, TH",
    "Beijing, CN",
    "Dubai, AE",
    "Hong Kong, HK",
    "Istanbul, TR",
    "Jakarta, ID",
    "Kuala Lumpur, MY",
    "Manila, PH",
    "Seoul, KR",
    "Shanghai, CN",
    "Singapore, SG",
    "Taipei, TW",
    "Tokyo, JP",

    "Chicago, Illinois, US",
    "Dallas, Texas, US",
    "Houston, Texas, US",
    "Los Angeles, California, US",
    "Miami, Florida, US",
    "New York, New York, US",
    "San Diego, California, US",
    "San Francisco, California, US",
    "Seattle, Washington, US",
    "Toronto, Ontario, CA",
    "Vancouver, British Columbia, CA",

    "Melbourne, Victoria, AU",
    "Sydney, New South Wales, AU",
    "Auckland, NZ",

    "Cape Town, ZA",
    "Johannesburg, ZA",
    "Nairobi, KE",

    "Doha, QA",
    "Riyadh, SA",
    "Abu Dhabi, AE"
};
    public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string city)
    {
        var root = await GetCityWeatherRootAsync(city);
        return MapCurrentWeather(root, city);
    }

    public async Task<ForecastResponse?> GetForecastAsync(string city)
    {
        var root = await GetFiveDayForecastRootAsync(city);
        var days = root.GetProperty("list")
            .EnumerateArray()
            .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).ToLocalTime().Date)
            .Take(7)
            .Select(group =>
            {
                var items = group.ToList();
                var summaryItem = items
                    .OrderBy(item => Math.Abs(DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).ToLocalTime().Hour - 12))
                    .First();
                var weather = summaryItem.GetProperty("weather")[0];
                var iconCode = weather.GetProperty("icon").GetString() ?? "01d";

                return new ForecastDay
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    MaxTemp = items.Max(item => item.GetProperty("main").GetProperty("temp_max").GetDouble()),
                    MinTemp = items.Min(item => item.GetProperty("main").GetProperty("temp_min").GetDouble()),
                    Description = weather.GetProperty("description").GetString() ?? string.Empty,
                    IconUrl = BuildIconUrl(iconCode, true),
                    Humidity = (int)Math.Round(items.Average(item => item.GetProperty("main").GetProperty("humidity").GetDouble())),
                    WindSpeed = items.Average(item => item.GetProperty("wind").GetProperty("speed").GetDouble())
                };
            })
            .ToList();

        return new ForecastResponse
        {
            City = root.GetProperty("city").GetProperty("name").GetString() ?? city,
            Days = days
        };
    }

    public async Task<HourlyResponse?> GetHourlyWeatherAsync(string city)
    {
        var root = await GetFiveDayForecastRootAsync(city);
        var hours = root.GetProperty("list")
            .EnumerateArray()
            .Take(12)
            .Select(item =>
            {
                var weather = item.GetProperty("weather")[0];
                var iconCode = weather.GetProperty("icon").GetString() ?? "01d";
                return new HourlyData
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).ToLocalTime().ToString("ddd HH:mm"),
                    Temperature = item.GetProperty("main").GetProperty("temp").GetDouble(),
                    Description = weather.GetProperty("description").GetString() ?? string.Empty,
                    IconUrl = BuildIconUrl(iconCode),
                    WindSpeed = item.GetProperty("wind").GetProperty("speed").GetDouble()
                };
            })
            .ToList();

        return new HourlyResponse
        {
            City = root.GetProperty("city").GetProperty("name").GetString() ?? city,
            Hours = hours
        };
    }

    public async Task<AirQualityResponse?> GetAirQualityAsync(string city)
    {
        var location = await GetCoordinatesForCityAsync(city);
        var root = await GetRootAsync($"{BaseUrl}/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={GetApiKey()}", "air quality");
        var listItem = root.GetProperty("list")[0];
        var components = listItem.GetProperty("components");
        var level = listItem.GetProperty("main").GetProperty("aqi").GetInt32();
        var pm25 = components.GetProperty("pm2_5").GetDouble();
        var pm10 = components.GetProperty("pm10").GetDouble();
        var no2 = components.GetProperty("no2").GetDouble();
        var o3 = components.GetProperty("o3").GetDouble();
        var aqi = CalculatePm25Aqi(pm25);

        return new AirQualityResponse
        {
            City = location.DisplayName,
            AQI = aqi,
            AQILevel = level,
            Status = GetAqiStatus(aqi),
            PM25 = pm25,
            PM10 = pm10,
            NO2 = no2,
            O3 = o3
        };
    }

    public async Task<WeatherResponse?> GetCurrentWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        var root = await GetRootAsync($"{BaseUrl}/data/2.5/weather?lat={latitude}&lon={longitude}&appid={GetApiKey()}&units=metric", "weather");
        var fallbackCity = await GetCityNameFromCoordinatesAsync(latitude, longitude);
        return MapCurrentWeather(root, fallbackCity);
    }

    public async Task<WeatherResponse?> GetCurrentWeatherFromIpAsync()
    {
        var location = await GetIpLocationAsync();
        return await GetCurrentWeatherByCoordinatesAsync(location.Latitude, location.Longitude);
    }

    public async Task<IReadOnlyList<string>> SearchCitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<string>();
        }

        var normalizedQuery = query.Trim();
        var root = await GetRootAsync($"{BaseUrl}/geo/1.0/direct?q={Uri.EscapeDataString(normalizedQuery)}&limit=10&appid={GetApiKey()}", "city suggestions");
        if (root.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<string>();
        }

        var suggestions = root.EnumerateArray()
            .Select(item =>
            {
                var name = item.GetProperty("name").GetString() ?? string.Empty;
                var country = item.TryGetProperty("country", out var countryProp) ? countryProp.GetString() : string.Empty;
                var state = item.TryGetProperty("state", out var stateProp) ? stateProp.GetString() : string.Empty;

                return string.Join(", ",
                    new[] { name, state, country }
                        .Where(value => !string.IsNullOrWhiteSpace(value)));
            })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Concat(PopularCities.Where(city => city.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(city => GetAutocompleteScore(city, normalizedQuery))
            .ThenBy(city => city, StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToList();

        return suggestions;
    }

    private async Task<JsonElement> GetCityWeatherRootAsync(string city)
    {
        return await GetRootAsync($"{BaseUrl}/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={GetApiKey()}&units=metric", "weather");
    }

    private async Task<JsonElement> GetFiveDayForecastRootAsync(string city)
    {
        return await GetRootAsync($"{BaseUrl}/data/2.5/forecast?q={Uri.EscapeDataString(city)}&appid={GetApiKey()}&units=metric", "forecast");
    }

    private async Task<CityCoordinates> GetCoordinatesForCityAsync(string city)
    {
        var root = await GetRootAsync($"{BaseUrl}/geo/1.0/direct?q={Uri.EscapeDataString(city)}&limit=1&appid={GetApiKey()}", "location");
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
        {
            throw new WeatherServiceException(404, $"City '{city}' was not found.");
        }

        var location = root[0];
        var name = location.GetProperty("name").GetString() ?? city;
        var country = location.TryGetProperty("country", out var countryProp) ? countryProp.GetString() : null;

        return new CityCoordinates(
            location.GetProperty("lat").GetDouble(),
            location.GetProperty("lon").GetDouble(),
            string.IsNullOrWhiteSpace(country) ? name : $"{name}, {country}");
    }

    private async Task<string> GetCityNameFromCoordinatesAsync(double latitude, double longitude)
    {
        var root = await GetRootAsync($"{BaseUrl}/geo/1.0/reverse?lat={latitude}&lon={longitude}&limit=1&appid={GetApiKey()}", "reverse geocoding");
        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            return root[0].GetProperty("name").GetString() ?? "Current Location";
        }

        return "Current Location";
    }

    private async Task<IpLocation> GetIpLocationAsync()
    {
        var root = await GetRootAsync(IpApiUrl, "IP location");
        var status = root.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : null;

        if (!string.Equals(status, "success", StringComparison.OrdinalIgnoreCase))
        {
            throw new WeatherServiceException(502, "Unable to detect location from IP right now.");
        }

        return new IpLocation(
            root.GetProperty("city").GetString() ?? "Current Location",
            root.GetProperty("lat").GetDouble(),
            root.GetProperty("lon").GetDouble());
    }

    private async Task<JsonElement> GetRootAsync(string url, string operationName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WeatherServiceException(404, $"The requested {operationName} data was not found.");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new WeatherServiceException(401, "Invalid OpenWeather API key. Update backend/appsettings.json with a valid key.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new WeatherServiceException(502, $"OpenWeather request failed while retrieving {operationName} data.");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch (WeatherServiceException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new WeatherServiceException(503, "Unable to reach the weather service right now. Please try again in a moment.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new WeatherServiceException(504, "The weather service took too long to respond.", ex);
        }
        catch (JsonException ex)
        {
            throw new WeatherServiceException(502, "Received an unexpected response from the weather service.", ex);
        }
    }

    private string GetApiKey()
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new WeatherServiceException(500, "OpenWeather API key is missing. Add it to backend/appsettings.json.");
        }

        return apiKey;
    }

    private static WeatherResponse MapCurrentWeather(JsonElement root, string fallbackCity)
    {
        var weather = root.GetProperty("weather")[0];
        var iconCode = weather.GetProperty("icon").GetString() ?? "01d";
        var main = root.GetProperty("main");

        return new WeatherResponse
        {
            City = root.GetProperty("name").GetString() ?? fallbackCity,
            Temperature = main.GetProperty("temp").GetDouble(),
            FeelsLike = main.GetProperty("feels_like").GetDouble(),
            Description = weather.GetProperty("description").GetString() ?? string.Empty,
            Humidity = main.GetProperty("humidity").GetInt32(),
            Pressure = main.GetProperty("pressure").GetInt32(),
            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
            IconUrl = BuildIconUrl(iconCode, true),
            Condition = weather.GetProperty("main").GetString() ?? string.Empty
        };
    }

    private static string BuildIconUrl(string iconCode, bool large = false)
    {
        return $"https://openweathermap.org/img/wn/{iconCode}@{(large ? "4x" : "2x")}.png";
    }

    // PM2.5 to AQI interpolation table used by the app.
    private static int CalculatePm25Aqi(double c)
    {
        (double Clow, double Chigh, int Ilow, int Ihigh)[] bands =
        [
            (0.0, 12.0, 0, 50),
            (12.1, 35.4, 51, 100),
            (35.5, 55.4, 101, 150),
            (55.5, 150.4, 151, 200),
            (150.5, 250.4, 201, 300),
            (250.5, 350.4, 301, 400),
            (350.5, 500.4, 401, 500)
        ];
        return InterpolateAqi(c, bands);
    }

    private static int InterpolateAqi(double concentration, (double Clow, double Chigh, int Ilow, int Ihigh)[] bands)
    {
        foreach (var (Clow, Chigh, Ilow, Ihigh) in bands)
        {
            if (concentration >= Clow && concentration <= Chigh)
            {
                var aqi = ((Ihigh - Ilow) / (Chigh - Clow)) * (concentration - Clow) + Ilow;
                return (int)Math.Floor(aqi);
            }
        }
        return concentration < 0 ? 0 : 500;
    }

    private static string GetAqiStatus(int aqi)
    {
        return aqi switch
        {
            <= 50 => "Good",
            <= 100 => "Moderate",
            <= 150 => "Unhealthy for Sensitive Groups",
            <= 200 => "Unhealthy",
            <= 300 => "Very Unhealthy",
            _ => "Hazardous"
        };
    }

    private static int GetAutocompleteScore(string city, string query)
    {
        var normalizedCity = city.ToLowerInvariant();
        var normalizedQuery = query.ToLowerInvariant();
        var primaryName = city.Split(',')[0].Trim().ToLowerInvariant();
        var score = 0;

        if (primaryName.Equals(normalizedQuery, StringComparison.OrdinalIgnoreCase))
        {
            score += 1000;
        }

        if (primaryName.StartsWith(normalizedQuery, StringComparison.OrdinalIgnoreCase))
        {
            score += 600;
        }

        if (normalizedCity.StartsWith(normalizedQuery, StringComparison.OrdinalIgnoreCase))
        {
            score += 250;
        }

        if (normalizedCity.Contains($", {normalizedQuery}", StringComparison.OrdinalIgnoreCase))
        {
            score += 40;
        }

        if (PopularCities.Contains(city, StringComparer.OrdinalIgnoreCase))
        {
            score += 200;
        }

        score -= Math.Abs(primaryName.Length - normalizedQuery.Length);
        return score;
    }

    private sealed record CityCoordinates(double Latitude, double Longitude, string DisplayName);
    private sealed record IpLocation(string City, double Latitude, double Longitude);
}
