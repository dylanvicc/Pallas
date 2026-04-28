const BASE_URL = import.meta.env.VITE_API_URL;

export async function api(path, options = {}) {

    const token = localStorage.getItem("token");

    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {})
    };

    const url = path.startsWith("http")
        ? path
        : `${BASE_URL}${path}`;

    const response = await fetch(url, {
        ...options,
        headers
    });

    return response;
}

export async function handleApiResponse(response) {
    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Request failed");
    }
    return response.json();
}
