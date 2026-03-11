export function hideForecast() {
    const hourlySection = document.getElementById("hourlySection");
    const forecastSection = document.getElementById("forecastSection");

    if (hourlySection) {
        hourlySection.classList.add("hidden");
    }

    if (forecastSection) {
        forecastSection.classList.add("hidden");
    }
}

export function renderHourlyForecast(hourlyData) {
    const container = document.getElementById("hourlyForecast");
    if (!container) return;

    container.innerHTML = "";

    const hours = Array.isArray(hourlyData?.hours) ? hourlyData.hours : [];

    hours.forEach((hour) => {
        const card = document.createElement("div");
        card.className = "hourly-card";

        card.innerHTML = `
            <div class="time">${hour.time}</div>
            <img src="${hour.iconUrl}" alt="${hour.description}">
            <div><strong>${Math.round(hour.temperature)}°C</strong></div>
            <div>${hour.description}</div>
            <div class="forecast-meta">Wind ${Math.round(hour.windSpeed)} m/s</div>
        `;

        container.appendChild(card);
    });
}

export function renderDailyForecast(forecastData) {
    const container = document.getElementById("forecastGrid");
    if (!container) return;

    container.innerHTML = "";

    const days = Array.isArray(forecastData?.days) ? forecastData.days : [];

    days.forEach((day) => {
        const card = document.createElement("div");
        card.className = "forecast-card";

        const date = new Date(day.date);
        const dayName = date.toLocaleDateString("en-US", {
            weekday: "short",
            month: "short",
            day: "numeric",
        });

        card.innerHTML = `
            <div class="forecast-day">${dayName}</div>
            <img src="${day.iconUrl}" alt="${day.description}">
            <div><strong>${Math.round(day.maxTemp)}° / ${Math.round(day.minTemp)}°</strong></div>
            <div>${day.description}</div>
            <div class="forecast-meta">Humidity ${day.humidity}%</div>
            <div class="forecast-meta">Wind ${Math.round(day.windSpeed)} m/s</div>
        `;

        container.appendChild(card);
    });
}
