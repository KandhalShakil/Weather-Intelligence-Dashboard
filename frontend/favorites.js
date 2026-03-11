const STORAGE_KEY = "weatherAppFavorites";

export function getFavorites() {
    try {
        const stored = localStorage.getItem(STORAGE_KEY);
        const parsed = stored ? JSON.parse(stored) : [];
        return Array.isArray(parsed) ? parsed : [];
    } catch {
        return [];
    }
}

export function addFavorite(city) {
    const normalized = city?.trim();
    if (!normalized) return;

    const favorites = getFavorites();
    const exists = favorites.some((item) => item.toLowerCase() === normalized.toLowerCase());

    if (!exists) {
        favorites.push(normalized);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(favorites));
    }
}

export function removeFavorite(city) {
    const favorites = getFavorites().filter(
        (item) => item.toLowerCase() !== city.toLowerCase()
    );

    localStorage.setItem(STORAGE_KEY, JSON.stringify(favorites));
}

export function renderFavorites(container, favorites, onSelectCity, onRemoveCity) {
    container.innerHTML = "";

    if (!favorites.length) {
        container.innerHTML = '<span class="city-badge">No favorites yet</span>';
        return;
    }

    favorites.forEach((city) => {
        const selectBtn = document.createElement("button");
        selectBtn.type = "button";
        selectBtn.className = "favorite-city-btn";
        selectBtn.textContent = city;
        selectBtn.addEventListener("click", () => onSelectCity(city));

        const removeBtn = document.createElement("button");
        removeBtn.type = "button";
        removeBtn.className = "remove-favorite-btn";
        removeBtn.textContent = "x";
        removeBtn.title = `Remove ${city}`;
        removeBtn.addEventListener("click", (event) => {
            event.stopPropagation();
            onRemoveCity(city);
        });

        const item = document.createElement("div");
        item.style.display = "inline-flex";
        item.style.gap = "0.35rem";
        item.append(selectBtn, removeBtn);

        container.appendChild(item);
    });
}
