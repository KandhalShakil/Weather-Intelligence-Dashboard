using System.Text.Json.Serialization;

namespace backend.Models;

public class WeatherResponse
{
    public string City { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public int Pressure { get; set; }
    public string Condition { get; set; } = string.Empty;
}

public class ForecastResponse
{
    public string City { get; set; } = string.Empty;
    public List<ForecastDay> Days { get; set; } = new();
}

public class ForecastDay
{
    public string Date { get; set; } = string.Empty;
    public double MaxTemp { get; set; }
    public double MinTemp { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
}

public class HourlyResponse
{
    public string City { get; set; } = string.Empty;
    public List<HourlyData> Hours { get; set; } = new();
}

public class HourlyData
{
    public string Time { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public double WindSpeed { get; set; }
}

public class AirQualityResponse
{
    public string City { get; set; } = string.Empty;
    public int AQI { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AQILevel { get; set; }

    [JsonPropertyName("pm25")]
    public double PM25 { get; set; }

    [JsonPropertyName("pm10")]
    public double PM10 { get; set; }

    [JsonPropertyName("no2")]
    public double NO2 { get; set; }

    [JsonPropertyName("o3")]
    public double O3 { get; set; }
}
