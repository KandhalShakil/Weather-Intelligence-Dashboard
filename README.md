# Weather Web Application

Weather dashboard built with ASP.NET Core Web API on the backend and plain HTML, CSS, and JavaScript on the frontend.

## Folder Structure

```text
backend/
  Controllers/
    WeatherController.cs
  Models/
    WeatherModels.cs
  Services/
    WeatherService.cs
    WeatherServiceException.cs
  Program.cs
  appsettings.json

frontend/
  index.html
  style.css
  script.js
  weatherService.js
  forecast.js
  favorites.js
```

## Features

- city search with backend autocomplete
- current weather card
- current location weather via Geolocation API
- daily forecast cards
- hourly forecast blocks
- air quality index and pollutant breakdown
- favorite cities stored in LocalStorage
- recent searches stored in LocalStorage
- loading state and error state
- weather-condition animations for clear, rain, clouds, and snow
- permanent dark theme UI

## Backend Endpoints

- `GET /api/weather/{city}`
- `GET /api/weather/forecast/{city}`
- `GET /api/weather/hourly/{city}`
- `GET /api/weather/airquality/{city}`
- `GET /api/weather/location?lat={lat}&lon={lon}`
- `GET /api/weather/autocomplete?query={text}`

## Example Responses

### Current Weather

```json
{
  "city": "London",
  "temperature": 9.38,
  "feelsLike": 6.83,
  "description": "scattered clouds",
  "humidity": 80,
  "windSpeed": 4.97,
  "iconUrl": "https://openweathermap.org/img/wn/03n@4x.png",
  "pressure": 1012,
  "condition": "Clouds"
}
```

### Air Quality

```json
{
  "city": "London, GB",
  "aqi": 5,
  "status": "Good",
  "aqiLevel": 2,
  "pm25": 1.11,
  "pm10": 1.84,
  "no2": 3.97,
  "o3": 93.25
}
```

### Forecast

```json
{
  "city": "London",
  "days": [
    {
      "date": "2026-03-11",
      "maxTemp": 11.07,
      "minTemp": 8.08,
      "description": "light rain",
      "iconUrl": "https://openweathermap.org/img/wn/10n@4x.png",
      "humidity": 70,
      "windSpeed": 5.61
    }
  ]
}
```

### Autocomplete

```json
[
  "London, GB",
  "London, Ontario, CA",
  "Londonderry, GB"
]
```

## UI Notes

- dark-only interface with glass-style cards and blue accent actions
- centered search bar with backend-powered autocomplete suggestions
- current weather card is the main focus area
- AQI, favorites, and recent searches are grouped in the right column on larger screens
- hourly forecast scrolls horizontally on smaller screens
- daily forecast uses a responsive grid

## OpenWeather API Key

Set the key in backend/appsettings.json:

```json
{
  "OpenWeather": {
    "ApiKey": "YOUR_API_KEY"
  }
}
```

Get a key from https://openweathermap.org/api.

## Run Locally

### Backend

```powershell
cd backend
dotnet restore
dotnet run
```

Backend listens on `http://localhost:5235`.

### Frontend

Open `frontend/index.html` with Live Server or any local static server, typically on `http://localhost:5500`.

## Notes

- the current API key supports OpenWeather current weather, air quality, and the standard `data/2.5/forecast` feed
- OpenWeather paid daily forecast endpoints returned `401 Unauthorized`, so the app aggregates the supported forecast feed into daily cards
- backend autocomplete ranking prioritizes common cities like London, Tokyo, New York, Ahmedabad, and Paris more consistently

## ❓ Troubleshooting

### **CORS Error**

**Error**: `Access to fetch at 'http://localhost:5235/...' blocked by CORS policy`

**Solution**: Ensure backend is running and CORS policy includes `http://localhost:5173`

### **API Key Invalid**

**Error**: `{"error": "Invalid API key"}`

**Solution**: 
1. Get new key from [openweathermap.org](https://openweathermap.org/api)
2. Update `appsettings.json`
3. Restart backend

### **Frontend Won't Connect**

**Check**:
- Is backend running on `http://localhost:5235`?
- Is frontend running on `http://localhost:5173`?
- Are ports 5235 and 5173 available?

### **City Not Found**

**Note**: Some countries require postal codes:
- Try **"London, GB"** instead of just **"London"**
- Use **"New York, US"** for US cities

---

## 📦 Dependencies

### **Backend**
- ASP.NET Core 10.0
- System.Net.Http (built-in)

### **Frontend**
- **Vite** 5.3+ (build tool)
- **TypeScript** 5.4+ (language)
- No runtime dependencies (pure vanilla TypeScript)

---

## 🎓 Best Practices Implemented

✅ **Clean Architecture**: Separated concerns (controller, service, model)  
✅ **Error Handling**: User-friendly error messages with fallbacks  
✅ **Type Safety**: Full TypeScript typing (frontend)  
✅ **CORS**: Properly configured for cross-origin requests  
✅ **UI/UX**: Loading states, error messages, responsive design  
✅ **Configuration**: Externalized API key in appsettings  
✅ **Modern Stack**: ASP.NET Core + Vite + TypeScript  

---

## 📄 License

Open source – use freely for learning and projects.

---

**Happy coding! 🌤️☀️**
