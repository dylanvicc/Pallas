const API_URL = import.meta.env.VITE_API_URL;

export async function api(path, options = {}) {

    const token = localStorage.getItem("token");

    return fetch(`${API_URL}${path}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            Authorization: token ? `Bearer ${token}` : "",
            ...options.headers
        }
    });
}