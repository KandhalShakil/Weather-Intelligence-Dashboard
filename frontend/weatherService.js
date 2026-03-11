const API_BASE_URL = 'https://weather-intelligence-dashboard.onrender.com/api/weather';

async function request(path) {
    const response = await fetch(`${API_BASE_URL}${path}`);
    if (!response.ok) {
        let errorMessage = 'Unable to fetch weather data.';
        try {
            const payload = await response.json();
            errorMessage = payload.error || errorMessage;
        } catch {
            errorMessage = response.status === 404 ? 'City not found.' : errorMessage;
        }

        throw new Error(errorMessage);
    }

    return response.json();
}

export const WeatherService = {
    getCurrentWeather(city) {
        return request(`/${encodeURIComponent(city)}`);
    },
    getForecast(city) {
        return request(`/forecast/${encodeURIComponent(city)}`);
    },
    getHourlyForecast(city) {
        return request(`/hourly/${encodeURIComponent(city)}`);
    },
    getAirQuality(city) {
        return request(`/airquality/${encodeURIComponent(city)}`);
    },
    getWeatherByCoordinates(lat, lon) {
        return request(`/location?lat=${encodeURIComponent(lat)}&lon=${encodeURIComponent(lon)}`);
    },
    getWeatherByIpLocation() {
        return request('/location/ip');
    },
    async getCityAutocomplete(query) {
        const normalizedQuery = query.trim();
        if (!normalizedQuery) {
            return [];
        }

        return request(`/autocomplete?query=${encodeURIComponent(normalizedQuery)}`);
    },

    // Backward compatibility aliases for older callers.
    getHourlyWeather(city) {
        return this.getHourlyForecast(city);
    },
    getSuggestions(query) {
        return this.getCityAutocomplete(query);
    },
};
