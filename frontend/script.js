import { WeatherService } from "./weatherService.js";
import {
    renderDailyForecast,
    renderHourlyForecast,
    hideForecast,
} from "./forecast.js";
import {
    getFavorites,
    addFavorite,
    removeFavorite,
    renderFavorites,
} from "./favorites.js";

const cityInput = document.getElementById("cityInput");
const searchBtn = document.getElementById("searchBtn");
const locationBtn = document.getElementById("locationBtn");
const favoriteBtn = document.getElementById("favoriteBtn");
const themeToggle = document.getElementById("themeToggle");
const localTimeEl = document.getElementById("localTime");

const loader = document.getElementById("loader");
const errorMsg = document.getElementById("errorMsg");
const currentWeatherSection = document.getElementById("currentWeather");
const airQualitySection = document.getElementById("airQuality");
const forecastSection = document.getElementById("forecastSection");
const hourlySection = document.getElementById("hourlySection");

const cityNameEl = document.getElementById("cityName");
const temperatureEl = document.getElementById("temperature");
const feelsLikeEl = document.getElementById("feelsLike");
const descriptionEl = document.getElementById("description");
const weatherIconEl = document.getElementById("weatherIcon");
const humidityEl = document.getElementById("humidity");
const windSpeedEl = document.getElementById("windSpeed");
const pressureEl = document.getElementById("pressure");
const weatherAnimationEl = document.getElementById("weatherAnimation");

const aqiValueEl = document.getElementById("aqiValue");
const aqiStatusEl = document.getElementById("aqiStatus");
const aqiMetaEl = document.getElementById("aqiMeta");
const pm25El = document.getElementById("pm25");
const pm10El = document.getElementById("pm10");
const no2El = document.getElementById("no2");
const o3El = document.getElementById("o3");

const autocompleteList = document.getElementById("autocompleteList");
const recentSearchesList = document.getElementById("recentSearchesList");
const favoritesList = document.getElementById("favoritesList");

const WEATHER_CLASSES = [
    "weather-clear",
    "weather-clouds",
    "weather-rain",
    "weather-thunder",
    "weather-snow",
    "weather-mist",
    "weather-night",
];

const THEME_KEY = "weather-theme";
let currentCity = "";
let autocompleteTimer = null;
let localTimeTimer = null;

function showLoader() {
    loader.classList.remove("hidden");
}

function hideLoader() {
    loader.classList.add("hidden");
}

function showError(message) {
    errorMsg.textContent = message;
    errorMsg.classList.remove("hidden");
}

function hideError() {
    errorMsg.textContent = "";
    errorMsg.classList.add("hidden");
}

function setVisibility(el, visible) {
    if (visible) {
        el.classList.remove("hidden");
    } else {
        el.classList.add("hidden");
    }
}

function clearWeatherBackgroundClasses() {
    document.body.classList.remove(...WEATHER_CLASSES);
}

function applyWeatherBackground(weather) {
    clearWeatherBackgroundClasses();

    const iconCode = weather.iconUrl?.split("/").pop()?.split("@")[0] ?? "";
    const isNight = iconCode.includes("n");
    const condition = (weather.mainCondition || "").toLowerCase();

    if (isNight) {
        document.body.classList.add("weather-night");
        return;
    }

    if (condition.includes("thunder")) {
        document.body.classList.add("weather-thunder");
        return;
    }

    if (condition.includes("rain") || condition.includes("drizzle")) {
        document.body.classList.add("weather-rain");
        return;
    }

    if (condition.includes("snow")) {
        document.body.classList.add("weather-snow");
        return;
    }

    if (condition.includes("cloud")) {
        document.body.classList.add("weather-clouds");
        return;
    }

    if (condition.includes("mist") || condition.includes("haze") || condition.includes("fog")) {
        document.body.classList.add("weather-mist");
        return;
    }

    document.body.classList.add("weather-clear");
}

function setTheme(theme) {
    const mode = theme === "light" ? "theme-light" : "theme-dark";
    document.body.classList.remove("theme-light", "theme-dark");
    document.body.classList.add(mode);
    localStorage.setItem(THEME_KEY, theme === "light" ? "light" : "dark");

    if (themeToggle) {
        themeToggle.textContent = theme === "light" ? "Dark Mode" : "Light Mode";
    }
}

function toggleTheme() {
    const isLight = document.body.classList.contains("theme-light");
    setTheme(isLight ? "dark" : "light");
}

function initializeTheme() {
    const storedTheme = localStorage.getItem(THEME_KEY);

    if (storedTheme === "light" || storedTheme === "dark") {
        setTheme(storedTheme);
        return;
    }

    setTheme("dark");
}

function getAqiStatusText(aqi) {
    switch (aqi) {
        case 1:
            return "Good";
        case 2:
            return "Fair";
        case 3:
            return "Moderate";
        case 4:
            return "Poor";
        case 5:
            return "Very Poor";
        default:
            return "Unknown";
    }
}

function getAqiBadgeColor(aqi) {
    if (aqi === 1 || aqi === 2) return "#22c55e";
    if (aqi === 3) return "#f59e0b";
    return "#ef4444";
}

function updateWeatherAnimation(condition) {
    const normalized = (condition || "").toLowerCase();
    const options = ["clear", "clouds", "rain", "drizzle", "thunderstorm", "snow", "mist", "haze", "fog"];

    for (const option of options) {
        weatherAnimationEl.classList.remove(`anim-${option}`);
    }

    const animation = options.find((option) => normalized.includes(option)) || "clear";
    weatherAnimationEl.classList.add(`anim-${animation}`);
}

function triggerReveal(el, delay = 0) {
    if (!el) return;

    setTimeout(() => {
        el.classList.add("reveal-in");
    }, delay);
}

function setStaticRevealSurfaces() {
    const surfaces = document.querySelectorAll(".reveal-surface");
    surfaces.forEach((surface, index) => {
        triggerReveal(surface, 90 * index);
    });
}

function updateLocalTime() {
    if (!localTimeEl) return;

    const now = new Date();
    const time = now.toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
    });
    const date = now.toLocaleDateString([], {
        weekday: "short",
        month: "short",
        day: "numeric",
    });

    localTimeEl.textContent = `${date}  |  Local time ${time}`;
}

function renderCurrentWeather(weather) {
    cityNameEl.textContent = weather.city;
    temperatureEl.textContent = `${Math.round(weather.temperature)}°C`;
    feelsLikeEl.textContent = `Feels like ${Math.round(weather.feelsLike)}°C`;
    descriptionEl.textContent = weather.description;
    humidityEl.textContent = `${weather.humidity}%`;
    windSpeedEl.textContent = `${weather.windSpeed.toFixed(1)} m/s`;
    pressureEl.textContent = `${weather.pressure} hPa`;

    weatherIconEl.src = weather.iconUrl;
    weatherIconEl.alt = `${weather.description} icon`;

    updateWeatherAnimation(weather.mainCondition);
    applyWeatherBackground(weather);

    setVisibility(currentWeatherSection, true);
    triggerReveal(currentWeatherSection, 0);
}

function renderAirQuality(aqiData) {
    const aqi = aqiData.aqi;
    const status = getAqiStatusText(aqi);
    const color = getAqiBadgeColor(aqi);

    aqiValueEl.textContent = aqi;
    aqiValueEl.style.color = color;
    aqiStatusEl.textContent = status;
    aqiStatusEl.style.color = color;
    aqiMetaEl.textContent = "Measured from your selected city coordinates";

    pm25El.textContent = `${Number(aqiData.pm25 ?? 0).toFixed(1)} µg/m3`;
    pm10El.textContent = `${Number(aqiData.pm10 ?? 0).toFixed(1)} µg/m3`;
    no2El.textContent = `${Number(aqiData.no2 ?? 0).toFixed(1)} µg/m3`;
    o3El.textContent = `${Number(aqiData.o3 ?? 0).toFixed(1)} µg/m3`;

    setVisibility(airQualitySection, true);
    triggerReveal(airQualitySection, 80);
}

function setFavoriteButtonState(isFavorite) {
    if (!favoriteBtn) return;

    favoriteBtn.textContent = isFavorite ? "Saved" : "Save";
    favoriteBtn.disabled = isFavorite;
}

function renderFavoritesList() {
    renderFavorites(favoritesList, getFavorites(), handleFavoriteClick, handleRemoveFavoriteClick);
    setFavoriteButtonState(getFavorites().includes(currentCity));
}

function getRecentSearches() {
    try {
        const raw = localStorage.getItem("recent-searches");
        const parsed = raw ? JSON.parse(raw) : [];
        return Array.isArray(parsed) ? parsed : [];
    } catch {
        return [];
    }
}

function setRecentSearches(items) {
    localStorage.setItem("recent-searches", JSON.stringify(items.slice(0, 8)));
}

function addRecentSearch(city) {
    const cleaned = city.trim();
    if (!cleaned) return;

    const unique = getRecentSearches().filter((item) => item.toLowerCase() !== cleaned.toLowerCase());
    unique.unshift(cleaned);
    setRecentSearches(unique);
}

function removeRecentSearch(city) {
    const next = getRecentSearches().filter((item) => item.toLowerCase() !== city.toLowerCase());
    setRecentSearches(next);
}

function renderRecentSearches() {
    const recent = getRecentSearches();
    recentSearchesList.innerHTML = "";

    if (recent.length === 0) {
        recentSearchesList.innerHTML = '<span class="city-badge">No recent searches</span>';
        return;
    }

    recent.forEach((city) => {
        const button = document.createElement("button");
        button.className = "recent-search-btn";
        button.type = "button";
        button.textContent = city;
        button.addEventListener("click", () => loadWeatherByCity(city));

        const remove = document.createElement("button");
        remove.className = "remove-recent-btn";
        remove.type = "button";
        remove.textContent = "x";
        remove.title = `Remove ${city}`;
        remove.addEventListener("click", (event) => {
            event.stopPropagation();
            removeRecentSearch(city);
            renderRecentSearches();
        });

        const wrapper = document.createElement("div");
        wrapper.style.display = "inline-flex";
        wrapper.style.gap = "0.35rem";
        wrapper.append(button, remove);

        recentSearchesList.appendChild(wrapper);
    });
}

function clearAutocomplete() {
    autocompleteList.innerHTML = "";
    autocompleteList.classList.add("hidden");
}

function renderAutocompleteSuggestions(cities) {
    autocompleteList.innerHTML = "";

    if (!cities.length) {
        const empty = document.createElement("div");
        empty.className = "autocomplete-empty";
        empty.textContent = "No city recommendations found";
        autocompleteList.appendChild(empty);
        autocompleteList.classList.remove("hidden");
        return;
    }

    cities.forEach((city) => {
        const parts = city.split(",").map((part) => part.trim()).filter(Boolean);
        const primary = parts[0] ?? city;
        const secondary = parts.slice(1).join(", ");

        const button = document.createElement("button");
        button.type = "button";
        button.className = "autocomplete-item";
        button.innerHTML = `
            <span class="autocomplete-main">${primary}</span>
            ${secondary ? `<span class="autocomplete-sub">${secondary}</span>` : ""}
        `;
        button.addEventListener("click", () => {
            cityInput.value = city;
            clearAutocomplete();
            loadWeatherByCity(city);
        });

        autocompleteList.appendChild(button);
    });

    autocompleteList.classList.remove("hidden");
}

async function updateAutocomplete(query) {
    const trimmed = query.trim();

    if (trimmed.length < 2) {
        clearAutocomplete();
        return;
    }

    try {
        const suggestions = await WeatherService.getCityAutocomplete(trimmed);
        renderAutocompleteSuggestions(suggestions);
    } catch {
        clearAutocomplete();
    }
}

async function loadWeatherBundle(city, source) {
    showLoader();
    hideError();

    try {
        const [weather, airQuality, dailyForecast, hourlyForecast] = await Promise.all([
            source(),
            WeatherService.getAirQuality(city),
            WeatherService.getForecast(city),
            WeatherService.getHourlyForecast(city),
        ]);

        currentCity = weather.city;
        renderCurrentWeather(weather);
        renderAirQuality(airQuality);
        renderDailyForecast(dailyForecast);
        renderHourlyForecast(hourlyForecast);

        addRecentSearch(currentCity);
        renderRecentSearches();
        renderFavoritesList();

        setVisibility(forecastSection, true);
        setVisibility(hourlySection, true);
    } catch (error) {
        setVisibility(currentWeatherSection, false);
        setVisibility(airQualitySection, false);
        setVisibility(forecastSection, false);
        setVisibility(hourlySection, false);
        currentWeatherSection.classList.remove("reveal-in");
        airQualitySection.classList.remove("reveal-in");
        forecastSection.classList.remove("reveal-in");
        hourlySection.classList.remove("reveal-in");
        hideForecast();

        const message = error instanceof Error ? error.message : "Unable to fetch weather data.";
        showError(message);
    } finally {
        hideLoader();
    }
}

async function loadWeatherByCity(city) {
    const normalized = city.trim();

    if (!normalized) {
        showError("Please enter a city name.");
        return;
    }

    await loadWeatherBundle(normalized, () => WeatherService.getCurrentWeather(normalized));
}

async function loadWeatherByLocation() {
    await loadWeatherBundle("location", () => WeatherService.getWeatherByIpLocation());
}

function handleSearch() {
    loadWeatherByCity(cityInput.value);
}

function handleFavoriteClick() {
    if (!currentCity) return;

    addFavorite(currentCity);
    renderFavoritesList();
}

function handleFavoriteCityClick(city) {
    triggerReveal(hourlySection, 160);
    triggerReveal(forecastSection, 230);
    loadWeatherByCity(city);
}

function handleRemoveFavoriteClick(city) {
    removeFavorite(city);
    renderFavoritesList();
}

function registerEvents() {
    searchBtn.addEventListener("click", handleSearch);
    locationBtn.addEventListener("click", loadWeatherByLocation);

    if (favoriteBtn) {
        favoriteBtn.addEventListener("click", handleFavoriteClick);
    }

    if (themeToggle) {
        themeToggle.addEventListener("click", toggleTheme);
    }

    cityInput.addEventListener("keydown", (event) => {
        if (event.key === "Enter") {
            clearAutocomplete();
            handleSearch();
        }
    });

    cityInput.addEventListener("input", () => {
        clearTimeout(autocompleteTimer);
        autocompleteTimer = setTimeout(() => {
            updateAutocomplete(cityInput.value);
        }, 220);
    });

    document.addEventListener("click", (event) => {
        const target = event.target;
        const clickedInsideAutocomplete = autocompleteList.contains(target);
        const clickedInput = target === cityInput;

        if (!clickedInsideAutocomplete && !clickedInput) {
            clearAutocomplete();
        }
    });
}

function initialize() {
    initializeTheme();
    registerEvents();
    setStaticRevealSurfaces();
    renderRecentSearches();
    renderFavoritesList();

    updateLocalTime();
    localTimeTimer = setInterval(updateLocalTime, 1000);

    loadWeatherByLocation();
}

initialize();
