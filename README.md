<div align="center">

# 🌤️ Weather Intelligence Dashboard

**A modern, full-stack weather app with real-time data, air quality, forecasts, and smart city search.**

</div>

---

## ✨ Features at a Glance

| Feature | Description |
|---|---|
| 🔍 **Smart City Search** | Backend autocomplete with ranked suggestions |
| 📍 **Auto Location** | Detects your location via IP on first load |
| 🌡️ **Current Weather** | Temperature, feels-like, humidity, wind, pressure |
| 📅 **7-Day Forecast** | Daily high/low, icons and wind summary |
| ⏱️ **Hourly Forecast** | 12-hour timeline with icons and conditions |
| 🌫️ **Air Quality Index** | PM2.5, PM10, NO₂, O₃ with AQI level status |
| ⭐ **Favorite Cities** | Save and reload your favorite locations |
| 🕐 **Recent Searches** | Quickly revisit past searches |
| 🌙 **Dark / Light Mode** | Theme toggle with localStorage persistence |
| 🎨 **Dynamic Backgrounds** | Weather-reactive gradient backgrounds |
| ⚡ **Live Local Time** | Real-time clock in the header |

---

## 🏗️ Tech Stack

### Backend 🖥️
- **ASP.NET Core Web API** (.NET 10)
- **OpenWeather API** — weather, forecast, geocoding, air quality
- **IP-API** — IP-based location detection
- Controller → Service → Model architecture

### Frontend 🌐
- **Plain HTML + CSS + JavaScript** (ES modules, no framework)
- Glassmorphism dark UI with animated weather states
- LocalStorage for favorites and recent searches

---

## 📁 Project Structure

```
📦 DotNet Project
├── 📂 backend/
│   ├── 📂 Controllers/
│   │   └── WeatherController.cs
│   ├── 📂 Models/
│   │   └── WeatherModels.cs
│   ├── 📂 Services/
│   │   ├── WeatherService.cs
│   │   └── WeatherServiceException.cs
│   ├── Program.cs
│   └── appsettings.json
│
└── 📂 frontend/
    ├── index.html
    ├── style.css
    ├── script.js
    ├── weatherService.js
    ├── forecast.js
    └── favorites.js
```

---

## 🚀 Getting Started

### 1️⃣ Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A free [OpenWeather API key](https://openweathermap.org/api)

### 2️⃣ Configure API Key

Open `backend/appsettings.json` and add your key:

```json
{
  "OpenWeather": {
    "ApiKey": "your_api_key_here"
  }
}
```

### 3️⃣ Run the Backend

```bash
dotnet run --project backend/backend.csproj
```

API starts at: `http://localhost:5235`

### 4️⃣ Open the Frontend

Open `frontend/index.html` directly in your browser.  
The dashboard will auto-detect your location and load weather immediately. 🌍

---

## 🔌 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/weather/{city}` | Current weather for a city |
| `GET` | `/api/weather/forecast/{city}` | 7-day daily forecast |
| `GET` | `/api/weather/hourly/{city}` | 12-hour hourly forecast |
| `GET` | `/api/weather/airquality/{city}` | AQI and pollutant data |
| `GET` | `/api/weather/location?lat=&lon=` | Weather by coordinates |
| `GET` | `/api/weather/location/ip` | Weather by auto-detected IP |
| `GET` | `/api/weather/autocomplete?query=` | City name suggestions |

---

## 📦 Example API Responses

<details>
<summary>🌡️ Current Weather</summary>

```json
{
  "city": "Ahmedabad",
  "temperature": 28.5,
  "feelsLike": 30.1,
  "description": "few clouds",
  "humidity": 42,
  "windSpeed": 3.6,
  "iconUrl": "https://openweathermap.org/img/wn/02d@4x.png",
  "pressure": 1010,
  "condition": "Clouds"
}
```
</details>

<details>
<summary>🌫️ Air Quality</summary>

```json
{
  "city": "Ahmedabad, IN",
  "aqi": 3,
  "status": "Moderate",
  "aqiLevel": 3,
  "pm25": 18.4,
  "pm10": 29.1,
  "no2": 12.3,
  "o3": 82.5
}
```
</details>

<details>
<summary>📅 7-Day Forecast (excerpt)</summary>

```json
{
  "city": "Ahmedabad",
  "days": [
    {
      "date": "2026-03-11",
      "maxTemp": 32.0,
      "minTemp": 20.5,
      "description": "sunny",
      "iconUrl": "https://openweathermap.org/img/wn/01d@4x.png",
      "humidity": 38,
      "windSpeed": 4.1
    }
  ]
}
```
</details>

---

## 🎨 UI Highlights

- 🌑 **Dark / Light** — persistent theme toggle
- 🌈 **Weather Backgrounds** — gradient shifts for clear, rain, clouds, storm, snow, mist, and night
- ✨ **Reveal Animations** — staggered card entrance on every search
- 💫 **Sheen Effect** — subtle light sweep on the weather card
- 🔵 **Pulse Ripple** — animated weather condition indicator dot
- 🕐 **Live Clock** — real-time date and time in the header

---

## 🛡️ License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2026 Kandhal Shakil

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

<div align="center">

Made with ❤️ by **Kandhal Shakil** &nbsp;|&nbsp; Powered by [OpenWeather](https://openweathermap.org/) 🌦️

</div>
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
